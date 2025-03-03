using FluentValidation;

namespace Talent;

public class CourseValidator : AbstractValidator<CreateCourseDto>
{

    public CourseValidator()
    {
        RuleFor(course => course.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(2).WithMessage("Title should be at least 2 chars.")
            .MaximumLength(50).WithMessage("Title can't exceeds 50 chars.");

        RuleFor(course => course.Description)
           .NotEmpty().WithMessage("Description is required.")
           .MinimumLength(2).WithMessage("Description should be at least 2 chars.")
           .MaximumLength(1000).WithMessage("Description can't exceeds 250 chars.");

    }

 
}
