using System.ComponentModel.DataAnnotations;

public class TagCategory
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    
    public List<TagCategoryTag> TagCategoryTags { get; set; } = new();
}
