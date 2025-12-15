using FluentValidation;

namespace LMS.Core.Commands.Admins.CreateAdminCommands;

public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand>
{
    public CreateAdminCommandValidator()
    {
        RuleFor(s => s.FullName)
        .NotEmpty()
        .WithMessage("Full name is required");

        RuleFor(s => s.UserName)
            .NotEmpty()
            .Must(u => !u.Contains(' '))
            .WithMessage("Username cannot contain spaces.");

        RuleFor(s => s.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email address.");

        RuleFor(s => s.PhoneNumber)
            .NotEmpty()
            .Matches(@"^01\d{9}$")
            .WithMessage("Phone number must start with '01' and contain 11 digits.");
    }
}
