using System.ComponentModel.DataAnnotations;

public class TagCategoryTag
{
    [Key]
    public int Id { get; set; }

    public int TagId { get; set; }
    public Tag? Tag { get; set; }

    public int TagCategoryId { get; set; }
    public TagCategory? TagCategory { get; set; }
}
