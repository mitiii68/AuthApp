using System.ComponentModel.DataAnnotations;

namespace AuthApp.Models
{
    public class FavoriteDocument
    {
        public int Id { get; set; }

        public string UserEmail { get; set; } = string.Empty;

        public int FileDocumentsId { get; set; }
        public FileDocuments? FileDocument { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
        public ICollection<FavoriteDocument> FavoriteDocuments { get; set; } = new List<FavoriteDocument>();
    }
}
