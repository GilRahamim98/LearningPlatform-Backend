using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class EnrollmentValidator : AbstractValidator<CreateEnrollmentDto>, IDisposable
{
    private readonly AcademiaXContext _db;

    public EnrollmentValidator(AcademiaXContext db)
    {
        _db = db;
        RuleFor(enrollment => enrollment.CourseId)
                .NotEmpty().WithMessage("CourseId is required")
                .Must(CourseExists).WithMessage("The specified CourseId does not exist.");
        RuleFor(enrollment => enrollment.UserId)
               .NotEmpty().WithMessage("UserId is required")
               .Must(UserExists).WithMessage("The specified UserId does not exist.");

    }
    private bool CourseExists(Guid courseId)
    {
        return _db.Courses.AsNoTracking().SingleOrDefault(course => course.Id == courseId) != null;
    }
    private bool UserExists(Guid userId)
    {
        return _db.Users.AsNoTracking().SingleOrDefault(user => user.Id == userId) != null;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
