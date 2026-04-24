using AuthApp.Models;
using System.ComponentModel.DataAnnotations;

public class Tag
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public int TagCategoryId { get; set; }
    public TagCategory?Category { get; set; }
    public List<FileTag> FileTags { get; set; } = new();
}