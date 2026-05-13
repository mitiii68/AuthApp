using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApp.Models
{
    public class ProjectDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }

        [ForeignKey(nameof(ProjectId))]
        public Project? Project { get; set; }

        public string? FileName  { get; set; }
        public string? FilePath  { get; set; }
        public string? Extension { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}
