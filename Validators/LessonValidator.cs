using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class LessonValidator : AbstractValidator<CreateLessonDto>,IDisposable
{
    private readonly AcademiaXContext _db;

    public LessonValidator(AcademiaXContext db)
    {
        _db = db;

        RuleFor(lesson => lesson.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(2).WithMessage("Title should be at least 2 chars.")
            .MaximumLength(50).WithMessage("Title can't exceeds 50 chars.");

        RuleFor(lesson => lesson.VideoUrl)
            .NotEmpty().WithMessage("VideoUrl is required")
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute)).WithMessage("Invalid video URL format");

        RuleFor(lesson => lesson.CourseId)
            .NotEmpty().WithMessage("CourseId is required")
            .Must(CourseExists).WithMessage("The specified CourseId does not exist.");

    }

    private bool CourseExists(Guid courseId)
    {
        return _db.Courses.AsNoTracking().SingleOrDefault(course => course.Id == courseId) != null;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
