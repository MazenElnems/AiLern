using FluentValidation;

namespace LMS.Application.Features.Auth.Commands.Login;

public class UserLoginByEmailAndPasswordCommandValidator : AbstractValidator<UserLoginByEmailAndPasswordCommand>
{
    public UserLoginByEmailAndPasswordCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
