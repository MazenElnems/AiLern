using FluentValidation;

namespace LMS.Application.Features.Attempts.Commands.SaveAttempt;

public class SaveAttemptCommandValidator : AbstractValidator<SaveAttemptCommand>
{
    public SaveAttemptCommandValidator()
    {
        RuleFor(c => c.AttemptId)
            .NotEmpty().WithMessage("AttemptId is required.");

        RuleFor(c => c.Answers)
            .NotEmpty().WithMessage("At least one answer is required.");

        RuleForEach(c => c.Answers)
            .ChildRules(ruleBuilder =>
            {
                ruleBuilder.RuleFor(a => a.QuestionId)
                    .NotEmpty().WithMessage("QuestionId is required for each answer.")
                    .NotNull().WithMessage("QuestionId cannot be null for each answer.");

                ruleBuilder.RuleFor(a => a.WrittenAnswer)
                    .MaximumLength(2000).WithMessage("WrittenAnswer cannot exceed 2000 characters.")
                    .When(a => !string.IsNullOrEmpty(a.WrittenAnswer));

                ruleBuilder.When(a => !a.OptionId.HasValue, () =>
                {
                    ruleBuilder.RuleFor(a => a.WrittenAnswer)
                        .NotNull().WithMessage("WrittenAnswer must be provided when OptionId is not specified.")
                        .NotEmpty().WithMessage("WrittenAnswer cannot be empty when OptionId is not specified.");
                });

                ruleBuilder.When(a => string.IsNullOrEmpty(a.WrittenAnswer), () =>
                {
                    ruleBuilder.RuleFor(a => a.OptionId)
                        .NotNull().WithMessage("OptionId must be provided when WrittenAnswer is not specified.")
                        .NotEmpty().WithMessage("OptionId cannot be empty when WrittenAnswer is not specified.");
                });
            });
    }
}
