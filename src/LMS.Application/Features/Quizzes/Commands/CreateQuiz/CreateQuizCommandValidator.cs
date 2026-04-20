using FluentValidation;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithMessage("Title is required.")
            .NotNull().WithMessage("Title Can't be null");

        RuleFor(c => c.AvailableFrom)
            .LessThan(c => c.AvailableUntil).WithMessage("AvailableFrom must be less than AvailableUntil.")
            .GreaterThan(DateTime.UtcNow).WithMessage("AvailableFrom must be in the future.");

        RuleFor(c => c.AttemptTimeLimit)
            .GreaterThan(0).WithMessage("AttemptTimeLimit must be greater than 0.");

        RuleFor(c => c.AttemptTimeLimit)
            .LessThanOrEqualTo(c => (int)(c.AvailableUntil - c.AvailableFrom).TotalMinutes)
            .WithMessage("AttemptTimeLimit must be less than or equal to the total available time.")
            .When(c => c.AvailableUntil > c.AvailableFrom);
            
        RuleFor(c => c.MaximumAttempts)
            .InclusiveBetween(1, 3).WithMessage("MaximumAttempts must be between 1 and 3.");
    }
}
