using FluentValidation;

namespace Talent;

public class LoginValidator : AbstractValidator<LoginUserDto>
{
    public LoginValidator()
    {
       
        RuleFor(login => login.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.")
            .MaximumLength(100).WithMessage("Email can't exceeds 100 chars.");

        RuleFor(login => login.Password)
           .NotEmpty().WithMessage("Password is required.")
           .MinimumLength(8).WithMessage("Password should be at least 8 chars.")
           .MaximumLength(250).WithMessage("Password can't exceeds 250 chars.");
    }
}
