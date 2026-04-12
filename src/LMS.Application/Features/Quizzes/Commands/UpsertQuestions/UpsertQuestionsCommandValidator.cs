using FluentValidation;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Commands.UpsertQuestions;

public class UpsertQuestionsCommandValidator : AbstractValidator<UpsertQuestionsCommand>
{
    public UpsertQuestionsCommandValidator()
    {
        RuleFor(c => c.QuizId)
            .NotEmpty().WithMessage("QuizId is required.");

        RuleFor(c => c.Questions)
            .NotEmpty().WithMessage("Questions are required.");

        RuleForEach(c => c.Questions)
            .ChildRules(q =>
            {
                q.RuleFor(q => q.QuestionText)
                    .NotEmpty().WithMessage("QuestionText is required.")
                    .NotNull().WithMessage("QuestionText cannot be null.")
                    .MaximumLength(2000).WithMessage("QuestionText cannot exceed 2000 characters.");

                q.RuleFor(q => q.Mark)
                    .GreaterThan(0).WithMessage("Mark must be greater than 0.")
                    .LessThanOrEqualTo(100).WithMessage("Mark must be less than or equal to 100.")
                    .NotNull().WithMessage("Mark cannot be null.");

                q.RuleFor(q => q.QuestionType)
                    .IsInEnum().WithMessage("Invalid QuestionType.")
                    .NotNull().WithMessage("QuestionType cannot be null.");

                q.RuleFor(q => q.Instructions)
                    .MaximumLength(1000).WithMessage("Instructions cannot exceed 1000 characters.")
                    .When(q => !string.IsNullOrEmpty(q.Instructions));

                q.RuleFor(q => q.Explanation)
                    .MaximumLength(1000).WithMessage("Explanation cannot exceed 1000 characters.")
                    .When(q => !string.IsNullOrEmpty(q.Explanation));

                q.When(q => q.QuestionType == QuestionType.TrueFalse || q.QuestionType == QuestionType.MCQ, () => 
                {
                    q.RuleFor(q => q.Options)
                        .NotEmpty().WithMessage("Options are required for this type of questions.")
                        .Must(options => options.Count(o => o.IsCorrect) == 1)
                        .WithMessage("Exactly one option must be marked as correct.");

                   q.RuleForEach(q => q.Options)
                        .ChildRules(o =>
                        {
                            o.RuleFor(opt => opt.OptionText)
                                .NotEmpty().WithMessage("OptionText is required.")
                                .NotNull().WithMessage("OptionText cannot be null.")
                                .MaximumLength(300).WithMessage("OptionText cannot exceed 300 characters.");
                        });
                });

                q.When(q => q.QuestionType == QuestionType.MCQ, () => 
                {
                    q.RuleFor(q => q.Options)
                        .Must(o => o.Count >= 3 && o.Count <= 5)
                        .WithMessage("MCQ questions must have between 3 and 5 options.");
                });

                q.When(q => q.QuestionType == QuestionType.TrueFalse, () => 
                {
                    q.RuleFor(q => q.Options)
                        .Must(o => o.Count == 2)
                        .WithMessage("True/False questions must have exactly 2 options.")
                        .Must(o => o.Any(op => op.OptionText.Equals("True", StringComparison.OrdinalIgnoreCase)))
                        .WithMessage("True/False questions must have an option with text 'True'.")
                        .Must(o => o.Any(op => op.OptionText.Equals("False", StringComparison.OrdinalIgnoreCase)))
                        .WithMessage("True/False questions must have an option with text 'False'.");
                });
            });
    }
}
