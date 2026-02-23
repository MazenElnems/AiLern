using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.Email)
            .NotNull().WithMessage("Can't be null")
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(c => c.NewPasswor)
           .MustBeValidPassword();
    }
}
