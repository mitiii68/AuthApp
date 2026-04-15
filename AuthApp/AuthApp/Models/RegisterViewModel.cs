using System.ComponentModel.DataAnnotations;
namespace AuthApp.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string? FullName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [MinLength(8)]
        public string? Password { get; set; }
    }
}