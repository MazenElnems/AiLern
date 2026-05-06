using FluentValidation;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Enums;

namespace LMS.Application.Common.Validators;



public class AIQuestionGeneratedResponseValidator: AbstractValidator<AIQuestionGeneratedResponse>
{
    public AIQuestionGeneratedResponseValidator()
    {
        RuleFor(x => x.Question)
            .NotEmpty().WithMessage("Question is required.");

        RuleFor(x => x.QuestionType)
            .IsInEnum().WithMessage("Invalid question type.");

        RuleFor(x => x.Difficulty)
            .IsInEnum().WithMessage("Invalid difficulty level.");

        When(x => x.QuestionType == QuestionType.Written, () =>
        {
            RuleFor(x => x.Answer)
                .NotEmpty().WithMessage("Answer is required for written questions.");
        });

        When(x => x.QuestionType == QuestionType.MCQ, () =>
        {
            RuleFor(x => x.Options)
                .NotNull()
                .Must(o => o.Count == 4)
                .WithMessage("MCQ must have exactly 4 options.")
                .Must(o => o.All(opt =>
                    opt.StartsWith("A", StringComparison.OrdinalIgnoreCase) ||
                    opt.StartsWith("B", StringComparison.OrdinalIgnoreCase) ||
                    opt.StartsWith("C", StringComparison.OrdinalIgnoreCase) ||
                    opt.StartsWith("D", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Each option must start with A, B, C, or D");

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage("Correct answer is required for MCQ.")
                .Must(x => x.StartsWith("A", StringComparison.OrdinalIgnoreCase) ||
                           x.StartsWith("B", StringComparison.OrdinalIgnoreCase) ||
                           x.StartsWith("C", StringComparison.OrdinalIgnoreCase) ||
                           x.StartsWith("D", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Correct answer must match one of the options.");
        });

        When(x => x.QuestionType == QuestionType.TrueFalse, () =>
        {
            RuleFor(x => x.Options)
                .NotNull()
                .Must(o => o.Count == 2)
                .WithMessage("True/False must have exactly 2 options.")
                .Must(o => o.All(opt =>
                    opt.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                    opt.Equals("False", StringComparison.OrdinalIgnoreCase)))
                .WithMessage("Options must be 'True' or 'False'.");

            RuleFor(x => x.CorrectAnswer)
                .NotEmpty().WithMessage("Correct answer is required.")
                .Must(x =>
                    x.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                    x.Equals("False", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Correct answer must be 'True' or 'False'.");
        });
    }
}


