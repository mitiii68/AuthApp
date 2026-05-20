namespace AuthApp.Models
{
    public class UserNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Type { get; set; } = "approval";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string? Url { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}