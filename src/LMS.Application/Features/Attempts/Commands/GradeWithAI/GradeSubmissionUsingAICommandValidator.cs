using FluentValidation;

namespace LMS.Application.Features.Attempts.Commands.GradeWithAI;

public class GradeSubmissionUsingAICommandValidator : AbstractValidator<GradeSubmissionUsingAICommand>
{
    public GradeSubmissionUsingAICommandValidator()
    {
        RuleFor(x => x.AttemptIds)
            .NotEmpty().WithMessage("At least one attempt ID must be provided.")
            .NotNull().WithMessage("Attempt IDs cannot be null.");
    }
}
