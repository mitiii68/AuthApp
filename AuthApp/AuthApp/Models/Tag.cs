using AuthApp.Models;
using System.ComponentModel.DataAnnotations;

public class Tag
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    // M:M с TagCategory через промежуточную таблицу
    public List<TagCategoryTag> TagCategoryTags { get; set; } = new();
    public List<FileTag> FileTags { get; set; } = new();
}
