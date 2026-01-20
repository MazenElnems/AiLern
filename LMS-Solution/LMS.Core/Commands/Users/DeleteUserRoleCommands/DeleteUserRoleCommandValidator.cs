using FluentValidation;
using LMS.Domain.Constants;

namespace LMS.Core.Commands.Users.DeleteUserRoleCommands;

public class DeleteUserRoleCommandValidator : AbstractValidator<DeleteUserRoleCommand>
{
    public DeleteUserRoleCommandValidator()
    {
        RuleFor(u => u.Role)
            .NotEmpty()
            .Must(r => new[] { UserRoles.Admin , UserRoles.Instructor, UserRoles.Student }.Contains(r))
            .WithMessage($"Invalid role. Choose one of the following: " +
            $"{UserRoles.Admin}, {UserRoles.Instructor}, or {UserRoles.Student}.");


    }
}
