using System.ComponentModel.DataAnnotations;

namespace Talent;

public class Course
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Missing Title")]
    [MinLength(2, ErrorMessage = "Title should be at least 2 chars.")]
    [MaxLength(50, ErrorMessage = "Title can't exceeds 50 chars.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Missing Description")]
    [MinLength(2, ErrorMessage = "Description should be at least 2 chars.")]
    [MaxLength(1000, ErrorMessage = "Description can't exceeds 1000 chars.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Missing CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;


}
