using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Validators;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(q => q.CourseId)
            .NotNull().WithMessage("CourseId can't be null.")
            .NotEmpty().WithMessage("CourseId is required.");

        RuleFor(q => q.Quiz)
            .SetValidator(new QuizRequestValidator());
    }
}
