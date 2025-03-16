using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talent;

public class Enrollment
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Missing UserId")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;


    [Required(ErrorMessage = "Missing CourseId")]
    public Guid CourseId { get; set; }

    [ForeignKey("CourseId")]
    public Course Course { get; set; } = null!;


    [Required(ErrorMessage = "Missing EnrolledAt")]
    public DateTime EnrolledAt { get; set; } = DateTime.Now;
}
