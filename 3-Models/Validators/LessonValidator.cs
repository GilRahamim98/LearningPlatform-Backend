using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class LessonValidator : AbstractValidator<CreateLessonDto>
{
    public LessonValidator()
    {
        RuleFor(lesson => lesson.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(2).WithMessage("Title should be at least 2 chars.")
            .MaximumLength(50).WithMessage("Title can't exceeds 50 chars.");

        RuleFor(lesson => lesson.VideoUrl)
            .NotEmpty().WithMessage("VideoUrl is required")
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("Invalid video URL format");

        RuleFor(lesson => lesson.CourseId)
            .NotEmpty().WithMessage("CourseId is required");
    }
}
