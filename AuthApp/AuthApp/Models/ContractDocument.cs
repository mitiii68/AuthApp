using AuthApp.Enums;

namespace AuthApp.Models
{
    public class ContractDocument
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public int? FileDocumentId { get; set; }
        public string? FullName { get; set; }
        public string? FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DocumentApprovalStatus ApprovalStatus { get; set; } = DocumentApprovalStatus.Draft;
        public DateTime? ApprovedAt { get; set; }
        public ICollection<DocumentApproval> Approvals { get; set; } = new List<DocumentApproval>();
    }
}
