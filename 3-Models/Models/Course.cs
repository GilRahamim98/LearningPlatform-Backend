using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talent;

public class Course
{
    [Key]
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Missing CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [InverseProperty("Course")]
    public ICollection<Lesson> Lessons { get;} = new List<Lesson>();

    [InverseProperty("Course")]
    public ICollection<Enrollment> Enrollments { get; } = new List<Enrollment>();

}
