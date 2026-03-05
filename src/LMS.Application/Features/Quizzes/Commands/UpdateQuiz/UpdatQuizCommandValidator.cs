using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Validators;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        Include(new QuizRequestValidator());
    }
}
