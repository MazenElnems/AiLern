using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Auth.Commands.ResetPassword;

public class UserPasswordResetCommandValidator : AbstractValidator<UserPasswordResetCommand>
{
    public UserPasswordResetCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid Email format");

        RuleFor(c => c.NewPassword)
            .MustBeValidPassword();
    }
}
