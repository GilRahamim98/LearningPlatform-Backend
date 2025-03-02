namespace Talent;

public class CreateLessonDto
{
    public string Title { get; set; } = null!;
    public string VideoUrl { get; set; } = null!;
    public Guid CourseId { get; set; }
}

