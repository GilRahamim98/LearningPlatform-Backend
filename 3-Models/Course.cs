using System.ComponentModel.DataAnnotations;

namespace Talent;

public class Course
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Missing CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;


}
