using AuthApp.Enums;

namespace AuthApp.DTOs
{
    // ─── Запуск согласования ───────────────────────────────────────────────────

    public class StartApprovalRequest
    {
        /// <summary>
        /// Упорядоченный список участников для последовательного согласования.
        /// Если null — берутся все участники договора в произвольном порядке (параллельно).
        /// </summary>
        public List<int>? OrderedParticipantIds { get; set; }
    }

    // ─── Действие участника (согласовать / отклонить) ─────────────────────────

    public class ApprovalDecisionRequest
    {
        public ApprovalStatus Decision { get; set; }   // Approved | Rejected
        public string? Comment { get; set; }
    }

    // ─── Отображение истории согласования ─────────────────────────────────────

    public class DocumentApprovalDto
    {
        public int Id { get; set; }
        public int ContractDocumentId { get; set; }
        public string? DocumentName { get; set; }
        public DocumentApprovalStatus DocumentStatus { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<ApprovalRowDto> Rows { get; set; } = new();
    }

    public class ApprovalRowDto
    {
        public int ApprovalId { get; set; }
        public int ParticipantId { get; set; }
        public string? FullName { get; set; }
        public int OrderIndex { get; set; }
        public ApprovalStatus Status { get; set; }
        public DateTime? ViewedAt { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? Comment { get; set; }
        /// <summary>Является ли эта строка текущим авторизованным пользователем</summary>
        public bool IsCurrentUser { get; set; }
        /// <summary>Должность участника</summary>
        public string? Position { get; set; }
    }

    // ─── Обзор согласования по договору (для JS /api/contracts/{id}/approval-overview) ─

    public class ContractApprovalOverviewDto
    {
        public int ContractDocumentId { get; set; }
        public string? DocumentName { get; set; }
        /// <summary>DocumentApprovalStatus as int: 0=Draft, 1=InApproval, 2=Approved, 3=Rejected</summary>
        public int ApprovalStatus { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<ApprovalRowDto> Rows { get; set; } = new();
    }
}
