using System;
namespace AuthApp.Models
{ public class User
    {
        public int UserId { get; set; }
        public string?FullName { get; set; }
        public string?Email { get; set; }
        public string?Login { get; set; }
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public string?PasswordHash { get; set; }
      public string? ConfirmationToken { get; set; }
        public string? ConfirmationCode { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetExpiresAt { get; set; }
        public int LoginCount { get; set; }
        public bool IsBlocked { get; set; } = false;
        public string? District { get; set;  }
        public string? RuralDistrict { get; set; }   
        public string? Settlement { get; set; }      
        public string? Street { get; set; }          
        public string? House { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }








    }
}