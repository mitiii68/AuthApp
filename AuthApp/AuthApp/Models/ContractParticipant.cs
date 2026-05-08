using AuthApp.Enums;

namespace AuthApp.Models
{
    public class ContractParticipant
    {
        public int Id { get; set; }
        public int ContractId {  get; set; }
        public Contract? Contract { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
