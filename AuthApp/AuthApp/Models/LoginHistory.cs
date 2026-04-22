namespace AuthApp.Models
{

    public class LoginHistory
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public DateTime LoginTime { get; set; }
        public string? IPAddress { get; set; }
    }
}