namespace Talent;

public class LessonPreviewDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public Guid CourseId { get; set; }
}
