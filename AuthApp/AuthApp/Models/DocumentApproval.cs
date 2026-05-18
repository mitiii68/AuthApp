using AuthApp.Enums;

namespace AuthApp.Models
{   
    public class DocumentApproval
    {
        public int Id { get; set; }

        public int ContractDocumentId { get; set; }
        public ContractDocument? ContractDocument { get; set; }
        public int ContractParticipantId { get; set; }
        public ContractParticipant? ContractParticipant { get; set; }
        public int OrderIndex { get; set; }
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        public DateTime? ViewedAt { get; set; }
        public DateTime? DecidedAt { get; set; }
        public string? Comment { get; set; }
    }
}
