using AuthApp.Data;
using AuthApp.DTOs;
using AuthApp.Enums;
using AuthApp.Models;
using AuthApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Services
{
    public interface IDocumentApprovalService
    {
        Task StartApprovalAsync(int contractDocumentId, StartApprovalRequest request, int initiatorUserId = 0, CancellationToken ct = default);
        Task MarkViewedAsync(int approvalId, int currentUserId, CancellationToken ct = default);
        Task MakeDecisionAsync(int approvalId, int currentUserId, ApprovalDecisionRequest request, CancellationToken ct = default);
        Task<DocumentApprovalDto> GetApprovalHistoryAsync(int contractDocumentId, CancellationToken ct = default);
        Task<List<ContractApprovalOverviewDto>> GetApprovalOverviewAsync(int contractId, int currentUserId, CancellationToken ct = default);
    }

    public class DocumentApprovalService : IDocumentApprovalService
    {
        private readonly AppDbContext _db;
        private readonly IHubContext<ApprovalNotificationHub> _hub;

        public DocumentApprovalService(AppDbContext db, IHubContext<ApprovalNotificationHub> hub)
        {
            _db = db;
            _hub = hub;
        }
        public async Task StartApprovalAsync(int contractDocumentId, StartApprovalRequest request, int initiatorUserId = 0, CancellationToken ct = default)
        {
            var document = await _db.ContractDocuments
                .Include(d => d.Contract)
                    .ThenInclude(c => c!.Participants)
                .FirstOrDefaultAsync(d => d.Id == contractDocumentId, ct)
                ?? throw new KeyNotFoundException($"Документ {contractDocumentId} не найден.");

            if (document.ApprovalStatus == DocumentApprovalStatus.InApproval)
                throw new InvalidOperationException("Согласование уже запущено.");

            var old = _db.DocumentApprovals.Where(a => a.ContractDocumentId == contractDocumentId);
            _db.DocumentApprovals.RemoveRange(old);

            var participants = document.Contract!.Participants.ToList();
            if (participants.Count == 0)
                throw new InvalidOperationException("У договора нет участников для согласования.");

            List<ContractParticipant> ordered;
            if (request.OrderedParticipantIds is { Count: > 0 })
            {
                ordered = request.OrderedParticipantIds
                    .Select(id => participants.FirstOrDefault(p => p.Id == id)
                                  ?? throw new KeyNotFoundException($"Участник {id} не входит в договор."))
                    .ToList();
            }
            else
            {
                ordered = participants;
            }

            for (int i = 0; i < ordered.Count; i++)
            {
                _db.DocumentApprovals.Add(new DocumentApproval
                {
                    ContractDocumentId = contractDocumentId,
                    ContractParticipantId = ordered[i].Id,
                    OrderIndex = request.OrderedParticipantIds is { Count: > 0 } ? i : 0,
                    Status = ApprovalStatus.Pending
                });
            }

            document.ApprovalStatus = DocumentApprovalStatus.InApproval;
            document.ApprovedAt = null;

            // Переводим договор в стадию "На согласовании"
            if (document.Contract != null &&
                document.Contract.Stage != ContractStage.OnApproval)
            {
                document.Contract.Stage = ContractStage.OnApproval;
            }

            await _db.SaveChangesAsync(ct);

            var contractId = document.ContractId;
            var docName = document.FullName ?? $"Документ #{contractDocumentId}";
            foreach (var participant in ordered)
            {
                if (participant.UserId == initiatorUserId) continue;

                _db.UserNotifications.Add(new UserNotification
                {
                    UserId    = participant.UserId,
                    Type      = "started",
                    Title     = "Запущено согласование",
                    Message   = $"Документ «{docName}» ожидает вашего согласования.",
                    Url       = $"/Contracts/Edit/{contractId}",
                    CreatedAt = DateTime.UtcNow
                });

                await _hub.SendApprovalNotification(participant.UserId, new ApprovalNotificationDto
                {
                    Type    = "started",
                    Title   = "Запущено согласование",
                    Message = $"Документ «{docName}» ожидает вашего согласования.",
                    Url     = $"/Contracts/Edit/{contractId}",
                    SentAt  = DateTime.UtcNow
                });
            }
            await _db.SaveChangesAsync(ct);
        }
        public async Task MarkViewedAsync(int approvalId, int currentUserId, CancellationToken ct = default)
        {
            var approval = await GetOwnApprovalAsync(approvalId, currentUserId, ct);

            if (approval.ViewedAt == null)
            {
                approval.ViewedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }
        public async Task MakeDecisionAsync(int approvalId, int currentUserId, ApprovalDecisionRequest request, CancellationToken ct = default)
        {
            if (request.Decision == ApprovalStatus.Pending)
                throw new ArgumentException("Нельзя выставить статус Pending вручную.");

            var approval = await GetOwnApprovalAsync(approvalId, currentUserId, ct);

            if (approval.Status != ApprovalStatus.Pending)
                throw new InvalidOperationException("Решение уже принято.");

            if (approval.OrderIndex > 0)
            {
                bool previousApproved = await _db.DocumentApprovals
                    .Where(a => a.ContractDocumentId == approval.ContractDocumentId
                             && a.OrderIndex == approval.OrderIndex - 1)
                    .AllAsync(a => a.Status == ApprovalStatus.Approved, ct);

                if (!previousApproved)
                    throw new InvalidOperationException("Предыдущий участник ещё не согласовал документ.");
            }

            approval.Status = request.Decision;
            approval.DecidedAt = DateTime.UtcNow;
            approval.Comment = request.Comment;

            if (approval.ViewedAt == null)
                approval.ViewedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            await RecalculateDocumentStatusAsync(approval.ContractDocumentId, ct);

            var doc2 = await _db.ContractDocuments
                .Include(d => d.Contract)
                .FirstOrDefaultAsync(d => d.Id == approval.ContractDocumentId, ct);
            if (doc2?.Contract?.ResponsibleUserId != null)
            {
                var decisionText = request.Decision == ApprovalStatus.Approved ? "согласовал" : "отклонил";
                var participantName = approval.ContractParticipant?.User?.FullName ?? "Участник";
                var notifMsg = $"{participantName} {decisionText} документ «{doc2.FullName ?? $"#{approval.ContractDocumentId}"}».";
                var notifUrl = $"/Contracts/Edit/{doc2.ContractId}";
                var recipientId = doc2.Contract.ResponsibleUserId.Value;

                _db.UserNotifications.Add(new UserNotification
                {
                    UserId    = recipientId,
                    Type      = "decided",
                    Title     = "Решение по согласованию",
                    Message   = notifMsg,
                    Url       = notifUrl,
                    CreatedAt = DateTime.UtcNow
                });
                await _db.SaveChangesAsync(ct);

                await _hub.SendApprovalNotification(recipientId, new ApprovalNotificationDto
                {
                    Type    = "decided",
                    Title   = "Решение по согласованию",
                    Message = notifMsg,
                    Url     = notifUrl,
                    SentAt  = DateTime.UtcNow
                });
            }
        }

        public async Task<DocumentApprovalDto> GetApprovalHistoryAsync(int contractDocumentId, CancellationToken ct = default)
        {
            var document = await _db.ContractDocuments
                .Include(d => d.Approvals)
                    .ThenInclude(a => a.ContractParticipant)
                        .ThenInclude(p => p!.User)
                            .ThenInclude(u => u!.Position)
                .FirstOrDefaultAsync(d => d.Id == contractDocumentId, ct)
                ?? throw new KeyNotFoundException($"Документ {contractDocumentId} не найден.");

            return new DocumentApprovalDto
            {
                Id = document.Id,
                ContractDocumentId = document.Id,
                DocumentName = document.FullName,
                DocumentStatus = document.ApprovalStatus,
                ApprovedAt = document.ApprovedAt,
                Rows = document.Approvals
                    .OrderBy(a => a.OrderIndex)
                    .ThenBy(a => a.Id)
                    .Select(a => new ApprovalRowDto
                    {
                        ApprovalId = a.Id,
                        ParticipantId = a.ContractParticipantId,
                        FullName = a.ContractParticipant?.User?.FullName,
                        Position = a.ContractParticipant?.User?.Position?.Name,
                        OrderIndex = a.OrderIndex,
                        Status = a.Status,
                        ViewedAt = a.ViewedAt,
                        DecidedAt = a.DecidedAt,
                        Comment = a.Comment
                    }).ToList()
            };
        }

        private async Task<DocumentApproval> GetOwnApprovalAsync(int approvalId, int currentUserId, CancellationToken ct)
        {
            var approval = await _db.DocumentApprovals
                .Include(a => a.ContractParticipant)
                    .ThenInclude(p => p!.User)
                .Include(a => a.ContractDocument)
                .FirstOrDefaultAsync(a => a.Id == approvalId, ct)
                ?? throw new KeyNotFoundException($"Запись согласования {approvalId} не найдена.");

            if (approval.ContractParticipant!.UserId != currentUserId)
                throw new UnauthorizedAccessException("Нет прав для данного действия.");

            if (approval.ContractDocument!.ApprovalStatus != DocumentApprovalStatus.InApproval)
                throw new InvalidOperationException("Документ не находится на согласовании.");

            return approval;
        }

        private async Task RecalculateDocumentStatusAsync(int contractDocumentId, CancellationToken ct)
        {
            var approvals = await _db.DocumentApprovals
                .Where(a => a.ContractDocumentId == contractDocumentId)
                .ToListAsync(ct);

            var document = await _db.ContractDocuments
                .Include(d => d.Contract)
                .FirstOrDefaultAsync(d => d.Id == contractDocumentId, ct)
                ?? throw new KeyNotFoundException();

            if (approvals.Any(a => a.Status == ApprovalStatus.Rejected))
            {
                document.ApprovalStatus = DocumentApprovalStatus.Rejected;
            }
            else if (approvals.All(a => a.Status == ApprovalStatus.Approved))
            {
                document.ApprovalStatus = DocumentApprovalStatus.Approved;
                document.ApprovedAt = DateTime.UtcNow;

                // Проверяем: все ли документы этого договора теперь согласованы?
                if (document.Contract != null)
                {
                    var allContractDocs = await _db.ContractDocument
                        .Where(d => d.ContractId == document.ContractId)
                        .ToListAsync(ct);

                    bool allApproved = allContractDocs.All(d =>
                        d.Id == contractDocumentId
                            ? true  // текущий только что стал Approved
                            : d.ApprovalStatus == DocumentApprovalStatus.Approved);

                    if (allApproved)
                    {
                        document.Contract.Stage = ContractStage.ApprovedPendingAcknowledgement;
                    }
                }
            }

            await _db.SaveChangesAsync(ct);
        }
       
        public async Task<List<ContractApprovalOverviewDto>> GetApprovalOverviewAsync(
            int contractId, int currentUserId, CancellationToken ct = default)
        {
            var documents = await _db.ContractDocument
                .Where(cd => cd.ContractId == contractId)
                .Include(cd => cd.Approvals)
                    .ThenInclude(a => a.ContractParticipant)
                        .ThenInclude(p => p!.User)
                            .ThenInclude(u => u!.Position)
                .OrderBy(cd => cd.Id)
                .ToListAsync(ct);

            return documents.Select(doc => new ContractApprovalOverviewDto
            {
                ContractDocumentId = doc.Id,
                DocumentName       = doc.FullName,
                ApprovalStatus     = (int)doc.ApprovalStatus,
                ApprovedAt         = doc.ApprovedAt,
                Rows = doc.Approvals
                    .OrderBy(a => a.OrderIndex)
                    .ThenBy(a => a.Id)
                    .Select(a => new ApprovalRowDto
                    {
                        ApprovalId    = a.Id,
                        ParticipantId = a.ContractParticipantId,
                        FullName      = a.ContractParticipant?.User?.FullName,
                        Position = a.ContractParticipant?.User?.Position?.Name,
                        OrderIndex    = a.OrderIndex,
                        Status        = a.Status,
                        ViewedAt      = a.ViewedAt,
                        DecidedAt     = a.DecidedAt,
                        Comment       = a.Comment,
                        IsCurrentUser = a.ContractParticipant?.UserId == currentUserId
                    }).ToList()
            }).ToList();
        }

    }
}
