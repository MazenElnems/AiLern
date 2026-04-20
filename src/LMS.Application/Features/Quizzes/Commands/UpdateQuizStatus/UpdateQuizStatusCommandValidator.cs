using FluentValidation;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuizStatus;

public class UpdateQuizStatusCommandValidator : AbstractValidator<UpdateQuizStatusCommand>
{
    public UpdateQuizStatusCommandValidator()
    {
        RuleFor(c => c.Status)
            .IsInEnum().WithMessage("Invalid Status value.");

        RuleFor(c => c.QuizId)
            .NotEmpty().WithMessage("Invalid QuizId value.");
    }
}
