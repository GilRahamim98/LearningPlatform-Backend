namespace Talent;

public class ProgressDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime WatchedAt { get; set; }
}

