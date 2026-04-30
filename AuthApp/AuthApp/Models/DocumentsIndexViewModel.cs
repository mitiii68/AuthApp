using System.Collections.Generic;
using System.Linq;

namespace AuthApp.Models
{
    public class DocumentsIndexViewModel
    {
        public IEnumerable<FileDocuments> Documents { get; set; } = Enumerable.Empty<FileDocuments>();
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public string? SortOrder { get; set; }
        public string? Search { get; set; }
        public List<int> SelectedTags { get; set; } = new();
        public IEnumerable<Tag> Tags { get; set; } = Enumerable.Empty<Tag>();
    }
}