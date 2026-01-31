using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LMS.Application.Features.Courses.Commands.RejectEnrollment
{
    public class RejectEnrollmentCommandValidator : AbstractValidator<RejectEnrollmentCommand>
    {
        public RejectEnrollmentCommandValidator()
        {
            RuleFor(e => e.Reason)
            .NotEmpty()
            .NotNull();
        }
    }
}
