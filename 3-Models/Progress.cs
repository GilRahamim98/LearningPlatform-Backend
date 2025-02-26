using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Talent;

public class Progress
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Missing UserId")]
    public Guid UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;


    [Required(ErrorMessage = "Missing LessonId")]
    public Guid LessonId { get; set; }

    [ForeignKey("LessonId")]
    public Lesson Lesson { get; set; } = null!;

    public DateTime? WatchedAt { get; set; }

}
