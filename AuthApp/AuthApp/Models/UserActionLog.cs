using System;
namespace AuthApp.Models
{
    public class UserActionLog
    {
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public string? Action { get; set; }
        public DateTime ActionTime { get; set; } = DateTime.Now;
    }
}