using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Validators;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(q => q.QuizId)
            .NotEmpty().WithMessage("QuizId is required.")
            .NotNull().WithMessage("QuizId cannot be null.");

        RuleFor(q => q.Quiz)
            .SetValidator(new QuizRequestValidator());
    }
}
