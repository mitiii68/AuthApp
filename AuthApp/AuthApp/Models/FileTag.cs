using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class FileTag
    {
        [Key]
        public int Id { get; set; }

        public int FileDocumentsId { get; set; }
        public FileDocuments? FileDocuments { get; set; }

        public int TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}