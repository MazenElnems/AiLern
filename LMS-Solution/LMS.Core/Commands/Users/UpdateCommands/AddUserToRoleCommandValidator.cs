using FluentValidation;
using LMS.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Users.UpdateCommands
{
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
}
