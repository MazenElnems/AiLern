using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Courses.Commands.RejectCourse
{
    public class RejectCourseCommandValidator : AbstractValidator<RejectCourseCommand>
    {
        public RejectCourseCommandValidator()
        {
            RuleFor(e => e.Reason)
            .NotEmpty()
            .NotNull();
        }
    }
}
