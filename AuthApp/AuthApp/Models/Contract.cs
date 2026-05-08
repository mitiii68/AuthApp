using AuthApp.Enums;

namespace AuthApp.Models
{
    public class Contract
    {
        public int Id {  get; set; }
        public string? FullName { get; set; }
        public string? ShortName { get; set; }
        public int? CounterpartyId { get; set; }
        public Counterparty? Counterparty { get; set; }
        public ContractStage? Stage { get; set; }
        public ContractType? Type {  get; set; }
        public string? ResponsibleFromCustomer { get; set; }
        public int? ResponsibleUserId { get; set; }
        public User? ResponsibleUser { get; set; }

        public decimal? AmountWithVat { get; set; }

        public DateTime? ConclusionDate { get; set; }
        public DateTime? ClosingDate { get; set; }
        public DateTime? ExecutionStartDate { get; set; }

        public string? ContractNumber { get; set; }

        public int? SourceContractId { get; set; }
        public Contract? SourceContract { get; set; }

        public string? ProjectId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public ICollection<ContractParticipant> Participants { get; set; } = new List<ContractParticipant>();

    }
}
