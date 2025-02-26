using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talent;

public class Lesson
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Missing CourseId")]
    public Guid CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;

    [Required(ErrorMessage = "Missing Title")]
    [MinLength(2, ErrorMessage = "Title should be at least 2 chars.")]
    [MaxLength(50, ErrorMessage = "Title can't exceeds 50 chars.")]
    public string Title { get; set; } = null!;


    [Required(ErrorMessage = "Missing VideoUrl")]
    [Url(ErrorMessage = "Invalid video URL format")]
    public string VideoUrl { get; set; } = null!;

}
