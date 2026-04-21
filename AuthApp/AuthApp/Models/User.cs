using System;
namespace AuthApp.Models
{ public class User
    {
        public int UserId { get; set; }
        public string?FullName { get; set; }
        public string?Email { get; set; }
        public string?Login { get; set; }
        public int RoleId { get; set; }
        public string?PasswordHash { get; set; }
      public string? ConfirmationToken { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? Role { get; set; }
        public bool IsConfirmed { get; set; }
        
       


    }
}