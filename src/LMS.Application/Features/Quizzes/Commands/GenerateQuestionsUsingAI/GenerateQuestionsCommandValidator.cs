using FluentValidation;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public class GenerateQuestionsCommandValidator : AbstractValidator<GenerateQuestionsCommand>
{
    public GenerateQuestionsCommandValidator()
    {
        RuleFor(x => x.QuestionsCount)
            .GreaterThan(0)
            .WithMessage("QuestionsCount must be greater than 0.");

        When(x => x.FileIds == null || !x.FileIds.Any(), () =>
        {
            RuleFor(x => x.NewUploadedFiles)
                .NotEmpty()
                .WithMessage("At least one file must be uploaded if no materials are selected.");
        });

        RuleFor(x => x.QuestionTypeCounts)
            .NotNull().WithMessage("Question types can't be null.")
            .NotEmpty().WithMessage("Question types must be specified.")
            .Must(types => types != null && types.Keys.All(k =>
                k == QuestionType.MCQ ||
                k == QuestionType.TrueFalse ||
                k == QuestionType.Written));

        When(x => x.QuestionDifficultyPercents != null, () =>
        {
            RuleFor(x => x.QuestionDifficultyPercents)
                .Must(x => x.Keys.All(k =>
                    k == QuestionDifficultyLevels.Easy ||
                    k == QuestionDifficultyLevels.Medium ||
                    k == QuestionDifficultyLevels.Hard))
                .WithMessage("Difficulty levels must be Easy, Medium, or Hard.");

            RuleFor(x => x.QuestionDifficultyPercents)
                .Must(HaveValidPercentages)
                .WithMessage("Difficulty percentages must sum to 100.");
        });
    }

    private bool HaveValidPercentages(Dictionary<QuestionDifficultyLevels, float> difficulties)
    {
        var total = difficulties.Values.Sum();
        return Math.Abs(total - 100f) < 0.01f;
    }
}