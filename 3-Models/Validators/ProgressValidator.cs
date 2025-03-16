using FluentValidation;

namespace Talent;

public class ProgressValidator : AbstractValidator<CreateProgressDto>
{
    public ProgressValidator()
    {
        RuleFor(progress => progress.LessonId)
        .NotEmpty().WithMessage("LessonId is required");
        RuleFor(progress => progress.UserId)
               .NotEmpty().WithMessage("UserId is required");
    }
}
