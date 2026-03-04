using FluentValidation;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandValidator : AbstractValidator<UpdateQuizCommand>
{
    public UpdateQuizCommandValidator()
    {
        RuleFor(q => q.Title)
            .NotNull().WithMessage("can't be null")
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("can't exceed 200 characters");

        RuleFor(q => q.Description)
            .NotNull().WithMessage("can't be null")
            .NotEmpty().WithMessage("Description is required");

        RuleFor(q => q.AvailableFrom)
            .NotEmpty().WithMessage("AvailableFrom date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("can't be in the past");

        RuleFor(q => q.AvailableUntil)
            .NotEmpty().WithMessage("AvailableUntil date is required")
            .GreaterThan(q => q.AvailableFrom)
            .WithMessage("must be after AvailableFrom");

        RuleFor(q => q.MaximumAttempts)
            .GreaterThan(0)
            .WithMessage("must be greater than 0");

        RuleFor(q => q.TotalPoints)
            .GreaterThan(0)
            .WithMessage("must be greater than 0");

    }
}
