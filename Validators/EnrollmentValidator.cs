using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class EnrollmentValidator : AbstractValidator<CreateEnrollmentDto>
{
    public EnrollmentValidator()
    {
        RuleFor(enrollment => enrollment.CourseId)
              .NotEmpty().WithMessage("CourseId is required");
        RuleFor(enrollment => enrollment.UserId)
               .NotEmpty().WithMessage("UserId is required");
    }
 
}
