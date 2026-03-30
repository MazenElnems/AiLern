using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Auth.Commands.ChangePassword;

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.NewPassword)
           .MustBeValidPassword();
    }
}
