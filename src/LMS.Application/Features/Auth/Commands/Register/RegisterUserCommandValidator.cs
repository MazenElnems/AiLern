using FluentValidation;
using LMS.Application.Common.Validators;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Auth.Commands.Register;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(s => s.FullName)
            .NotEmpty()
            .WithMessage("Full name is required");

        RuleFor(s => s.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .NotNull().WithMessage("UserName Can't be null")
            .Must(u => !u.Contains(' ')).WithMessage("Username cannot contain spaces.");

        RuleFor(s => s.Email)
            .NotEmpty().WithMessage("Email is required.")
            .NotNull().WithMessage("Email Can't be null")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(s => s.Role)
            .NotNull().WithMessage("Role can't be null.")
            .Must(r => r == Roles.Instructor || r == Roles.Student).WithMessage("Invalid Role name.");

        RuleFor(s => s.Password)
            .MustBeValidPassword();
    }
}
