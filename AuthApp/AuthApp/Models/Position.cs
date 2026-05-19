using System;
namespace AuthApp.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
