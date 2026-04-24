using System.ComponentModel.DataAnnotations;

public class TagCategory
{
    [Key]
    public int Id { get; set; }

    public string? Name { get; set; }

    public List<Tag> Tags { get; set; } = new();
}