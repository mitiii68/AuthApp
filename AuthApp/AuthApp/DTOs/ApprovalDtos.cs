using AuthApp.Enums;

namespace AuthApp.DTOs
{
    public class StartApprovalRequest
    {
        public List<int>? OrderedParticipantIds { get; set; }
    }

    public class ApprovalDecisionRequest
    {
        public ApprovalStatus Decision { get; set; }   
        public string? Comment { get; set; }
    }

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
        public bool IsCurrentUser { get; set; }
        public string? Position { get; set; }
    }

    public class ContractApprovalOverviewDto
    {
        public int ContractDocumentId { get; set; }
        public string? DocumentName { get; set; }
        public int ApprovalStatus { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public List<ApprovalRowDto> Rows { get; set; } = new();
    }
}
