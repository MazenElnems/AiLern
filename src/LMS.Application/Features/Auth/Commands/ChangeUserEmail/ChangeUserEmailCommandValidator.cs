using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Auth.Commands.ChangeUserEmail;

public class ChangeUserEmailCommandValidator : AbstractValidator<ChangeUserEmailCommand>
{
    public ChangeUserEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty()
            .WithMessage("New email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.");

        RuleFor(x => x.CurrentPassword) 
            .NotEmpty()
            .WithMessage("Current password is required.")
            .MustBeValidPassword()
            .WithMessage("Invalid password format.");
    }
}
