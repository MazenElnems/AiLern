using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Commands.Students.CreateCommands
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
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
           
            RuleFor(s => s.StudentId)
                .NotEmpty()
                .Must(s => s > 0 && s.ToString().Length == 9)
                .WithMessage("Student ID must be a 9-digit positive number.");
        }
    }
}
