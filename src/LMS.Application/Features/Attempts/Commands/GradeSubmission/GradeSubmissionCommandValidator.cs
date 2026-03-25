using FluentValidation;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionCommandValidator()
    {
        RuleFor(a => a.Status)
            .Must(s => s == AttemptStatus.Submitted || s == AttemptStatus.Reviewed)
            .WithMessage("Status must be either Submitted or Reviewed.");
    }
}
