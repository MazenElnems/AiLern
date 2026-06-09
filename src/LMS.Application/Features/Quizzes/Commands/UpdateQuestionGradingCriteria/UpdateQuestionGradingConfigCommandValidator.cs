using FluentValidation;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuestionGradingCriteria;

public class UpdateQuestionGradingConfigCommandValidator : AbstractValidator<UpdateQuestionGradingConfigCommand>
{
    public UpdateQuestionGradingConfigCommandValidator()
    {
        RuleFor(x => x.GradingConfigDto)
            .ChildRules(g =>
            {
                g.RuleFor(g => g.ModelAnswer)
                    .NotEmpty().WithMessage("ModelAnswer is required.")
                    .NotNull().WithMessage("ModelAnswer cannot be null.")
                    .MaximumLength(2000).WithMessage("ModelAnswer cannot exceed 2000 characters.");

                g.RuleForEach(g => g.Criteria)
                    .ChildRules(c =>
                    {
                        c.RuleFor(cr => cr.Criterion)
                            .NotEmpty().WithMessage("Criterion is required.")
                            .NotNull().WithMessage("Criterion cannot be null.")
                            .MaximumLength(1000).WithMessage("Criterion cannot exceed 1000 characters.");
                        c.RuleFor(cr => cr.Mark)
                            .GreaterThan(0).WithMessage("Mark must be greater than 0.");
                    });
            });
    }
}
