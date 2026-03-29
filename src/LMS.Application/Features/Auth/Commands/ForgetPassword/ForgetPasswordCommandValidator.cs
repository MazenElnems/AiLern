using FluentValidation;
using LMS.Application.Features.Auth.Commands.PasswordResetEmail;

namespace LMS.Application.Features.Auth.Commands.ForgetPassword;

public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotNull().WithMessage("Email can't be null.")
            .NotEmpty().WithMessage("Email is required.");
    }
}
