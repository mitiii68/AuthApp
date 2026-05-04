using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class FileDocuments
    {
        [Key]
        public int Id { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? Extension { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.Now;

        public List<FileTag> FileTags { get; set; } = new List<FileTag>();
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}