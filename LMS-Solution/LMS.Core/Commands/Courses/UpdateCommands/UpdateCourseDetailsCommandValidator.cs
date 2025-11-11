using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Commands.Courses.UpdateCommands
{
    public class UpdateCourseDetailsCommandValidator : AbstractValidator<UpdateCourseDetailsCommand>
    {
        public UpdateCourseDetailsCommandValidator()
        {
            RuleFor(c => c.Code).Length(5, 7)
                .WithMessage("Course code must be between 5 and 7 characters.");
        }
    }
}
