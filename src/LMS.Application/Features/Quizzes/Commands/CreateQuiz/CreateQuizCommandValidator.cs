using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Validators;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class UpdatQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public UpdatQuizCommandValidator()
    {
        Include(new QuizRequestValidator());
    }
}
