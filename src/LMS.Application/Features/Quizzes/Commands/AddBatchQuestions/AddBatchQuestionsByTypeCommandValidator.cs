using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Quizzes.Commands.AddBatchQuestions;

public class AddBatchQuestionsByTypeCommandValidator : AbstractValidator<AddBatchQuestionsByTypeCommand>
{
    public AddBatchQuestionsByTypeCommandValidator()
    {
        RuleForEach(x => x.Questions)
            .SetValidator(new AIQuestionGeneratedResponseValidator());
    }
}
