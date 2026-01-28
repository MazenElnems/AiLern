using FluentValidation;
using LMS.Domain.Constants;

namespace LMS.Core.Commands.Users.UpdateCommands;

public class AddUserToRoleCommandValidator : AbstractValidator<AddUserToRoleCommand>
{
    public AddUserToRoleCommandValidator()
    {
        RuleFor(u => u.Role)
            .NotEmpty()
            .Must(r => new[] { UserRoles.Admin, UserRoles.Instructor, UserRoles.Student }.Contains(r))
            .WithMessage($"Invalid role. Choose one of the following: " +
            $"{UserRoles.Admin}, {UserRoles.Instructor}, or {UserRoles.Student}.");
    }
}
