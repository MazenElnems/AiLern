using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.NewPassword)
           .NotNull().WithMessage("NewPassword is required.")
           .MustBeValidPassword();

        RuleFor(c => c.CurrentPassword)
            .NotNull().WithMessage("CurrentPassword is required.")
            .MustBeValidPassword();
    }
}
