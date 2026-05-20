using AuthApp.DTOs;
using AuthApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthApp.Controllers
{
    // ── Маршруты для конкретного документа ───────────────────────────────────
    [ApiController]
    [Route("api/contract-documents/{documentId:int}/approvals")]
    public class DocumentApprovalController : ControllerBase
    {
        private readonly IDocumentApprovalService _service;

        public DocumentApprovalController(IDocumentApprovalService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetHistory(int documentId, CancellationToken ct)
        {
            var result = await _service.GetApprovalHistoryAsync(documentId, ct);
            return Ok(result);
        }

        [HttpPost("start")]
        public async Task<IActionResult> Start(int documentId,
            [FromBody] StartApprovalRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _service.StartApprovalAsync(documentId, request, userId, ct);
            return Ok(new { message = "Согласование запущено." });
        }

        [HttpPost("{approvalId:int}/view")]
        public async Task<IActionResult> MarkViewed(int documentId, int approvalId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _service.MarkViewedAsync(approvalId, userId, ct);
            return Ok();
        }

        /// <summary>
        /// POST api/contract-documents/{documentId}/approvals/{approvalId}/decide
        /// Принять решение (согласовать / отклонить)
        /// </summary>
        [HttpPost("{approvalId:int}/decide")]
        public async Task<IActionResult> Decide(int documentId, int approvalId,
            [FromBody] ApprovalDecisionRequest request, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            await _service.MakeDecisionAsync(approvalId, userId, request, ct);
            return Ok(new { message = "Решение зафиксировано." });
        }

        // ─────────────────────────────────────────────────────────────────────

        private int GetCurrentUserId()
        {
            // Сначала пробуем JWT claim
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out var claimId))
                return claimId;

            // Fallback на сессию (приложение использует сессионную авторизацию)
            var sessionId = HttpContext.Session.GetString("UserId");
            if (sessionId != null && int.TryParse(sessionId, out var sessionUserId))
                return sessionUserId;

            throw new UnauthorizedAccessException("Пользователь не аутентифицирован.");
        }
    }

    // ── Маршрут обзора согласований по договору ───────────────────────────────
    [ApiController]
    [Route("api/contracts")]
    public class ContractApprovalOverviewController : ControllerBase
    {
        private readonly IDocumentApprovalService _service;

        public ContractApprovalOverviewController(IDocumentApprovalService service)
        {
            _service = service;
        }

        /// <summary>
        /// GET api/contracts/{contractId}/approval-overview
        /// Возвращает список всех документов договора с историей согласования каждого.
        /// Используется JS на страницах ApprovalHistory и _ApprovalSection.
        /// </summary>
        [HttpGet("{contractId:int}/approval-overview")]
        public async Task<IActionResult> GetOverview(int contractId, CancellationToken ct)
        {
            var userId = GetCurrentUserId();
            var result = await _service.GetApprovalOverviewAsync(contractId, userId, ct);
            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────────

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claim != null && int.TryParse(claim, out var claimId))
                return claimId;

            var sessionId = HttpContext.Session.GetString("UserId");
            if (sessionId != null && int.TryParse(sessionId, out var sessionUserId))
                return sessionUserId;

            // Если авторизация не настроена для API — возвращаем 0
            // (IsCurrentUser будет false для всех строк, кнопки не покажутся)
            return 0;
        }
    }
}
