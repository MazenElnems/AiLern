using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.Validators;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(q => q.QuestionText)
            .NotNull().WithMessage("Question text cannot be null.")
            .NotEmpty().WithMessage("Question text is required.")
            .MaximumLength(2000).WithMessage("Question text cannot exceed 2000 characters.");

        RuleFor(q => q.QuestionType)
            .NotNull().WithMessage("Question type is required.")
            .IsInEnum().WithMessage("Invalid question type.");

        RuleFor(q => q.Mark)
            .GreaterThan(0).WithMessage("Mark must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Mark cannot exceed 100.");

        RuleFor(q => q.Explanation)
            .MaximumLength(2000).WithMessage("Explanation cannot exceed 2000 characters.");

        RuleFor(q => q.Instructions)
            .MaximumLength(2000).WithMessage("Instructions cannot exceed 2000 characters.");

        When(q => q.QuestionType == QuestionType.Written, () =>
        {
            RuleFor(q => q.Options)
                .Must(o => o == null || !o.Any()).WithMessage("Options should be empty for written questions.");
        });

        When(q => q.QuestionType == QuestionType.TrueFalse, () =>
        {
            RuleFor(q => q.Options)
                .NotNull().WithMessage("Options are required for true/false questions.");

            RuleFor(q => q.Options)
                .Must(o => o?.Count == 2).WithMessage("True/False questions must have exactly 2 options.");

            RuleFor(q => q.Options)
                .Must(o => o?.Count(opt => opt.IsCorrect) == 1)
                .WithMessage("True/False questions must have exactly one correct option.");
        });

        When(q => q.QuestionType == QuestionType.MCQ, () =>
        {
            RuleFor(q => q.Options)
                .NotNull().WithMessage("Options are required for multiple choice questions.");

            RuleFor(q => q.Options)
                .Must(o => o?.Count >= 3 && o?.Count <= 5).WithMessage("Multiple choice questions must have between 3 and 5 options.");

            RuleFor(q => q.Options)
                .Must(o => o?.Count(opt => opt.IsCorrect) == 1)
                .WithMessage("Multiple choice questions must have exactly one correct option.");
        });

        When(q => q.Options != null, () =>
        {
            RuleForEach(q => q.Options)
                .SetValidator(new OptionRequestValidator());
        });
    }
}
