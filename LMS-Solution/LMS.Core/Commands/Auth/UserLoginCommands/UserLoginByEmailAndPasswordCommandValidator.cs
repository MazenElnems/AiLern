using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Auth.UserLoginCommands
{
    public class UserLoginByEmailAndPasswordCommandValidator : AbstractValidator<UserLoginByEmailAndPasswordCommand>
    {
        public UserLoginByEmailAndPasswordCommandValidator()
        {
            RuleFor(u => u.Email)
                .EmailAddress()
                .WithMessage("Invalid email address.");
        }
    }
}
