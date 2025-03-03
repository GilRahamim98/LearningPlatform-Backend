using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class RegisterValidator :AbstractValidator<RegisterUserDTO>, IDisposable
{
    private readonly AcademiaXContext _db;

    public RegisterValidator(AcademiaXContext db)
    {
        _db = db;

        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name should be at least 2 chars.")
            .MaximumLength(50).WithMessage("Name must be at most 50 characters long.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email should be vaild.")
            .Must(EmailExists).WithMessage("Email already exists.")
            .MaximumLength(100).WithMessage("Email can't exceeds 100 chars.");

        RuleFor(user => user.Password)
           .NotEmpty().WithMessage("Password is required.")
           .MinimumLength(8).WithMessage("Password should be at least 8 chars.")
           .Matches("[A-Z]").WithMessage("Password should contain at least one capital letter.")
           .Matches("[0-9]").WithMessage("Password should contain at least one digit.")
           .Matches("[^a-zA-Z0-9]").WithMessage("Password should contain at least one non-alphanumeric char.")
           .MaximumLength(250).WithMessage("Password can't exceeds 250 chars.");

        RuleFor(user => user.RoleId)
            .NotEmpty().WithMessage("RoleId is required.")
            .Must(role => role == 1 || role == 2).WithMessage("RoleId must be either 1 or 2");
    }

    private bool EmailExists(string email)
    {
        return !_db.Users.AsNoTracking().Any(u => u.Email == email.ToLower());
    }


    public void Dispose()
    {
        _db.Dispose();
    }
}
