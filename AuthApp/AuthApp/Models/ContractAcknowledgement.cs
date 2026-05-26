namespace AuthApp.Models
{
    public class ContractAcknowledgement
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int UserId { get; set; }
        public DateTime AcknowledgedAt { get; set; }

        public Contract? Contract { get; set; }
        public User? User { get; set; }
    }
}
