using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talent;

public class Lesson
{
    [Key]
    public Guid Id { get; set; }

    public Guid CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    public string Title { get; set; } = null!;


    [Required(ErrorMessage = "Missing VideoUrl")]
    [Url(ErrorMessage = "Invalid video URL format")]
    public string VideoUrl { get; set; } = null!;

}
