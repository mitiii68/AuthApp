using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Team { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Status { get; set; }

        public string? ResponsiblePerson { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ActualEndDate { get; set; }

        public string? Contractors { get; set; }

        public string? InterestSource { get; set; }

        public string? Employee { get; set; }

        public string? Department { get; set; }

        public string? WarrantyPeriod { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProjectGroup { get; set; }

        public DateTime? PlannedEndDate { get; set; }

        public decimal? Budget { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<ProjectContract> ProjectContracts { get; set; } = new();
    }
}
