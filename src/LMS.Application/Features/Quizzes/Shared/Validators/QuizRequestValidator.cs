using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.Validators
{
    public class QuizRequestValidator : AbstractValidator<QuizRequest>
    {
        public QuizRequestValidator()
        {
            RuleFor(q => q.Title)
                .NotNull().WithMessage("can't be null")
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("can't exceed 200 characters");

            RuleFor(q => q.Description)
                .MaximumLength(2000).WithMessage("can't exceed 2000 characters")
                .When(q => !string.IsNullOrEmpty(q.Description));

            RuleFor(q => q.AvailableFrom)
                .NotEmpty().WithMessage("AvailableFrom date is required")
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("can't be in the past");

            RuleFor(q => q.AvailableUntil)
                .NotEmpty().WithMessage("AvailableUntil date is required")
                .GreaterThan(q => q.AvailableFrom).WithMessage("must be after AvailableFrom");

            When(q => q.Status == QuizStatus.Scheduled, () =>
            {
                RuleFor(q => q.PublishedDate)
                    .NotNull().WithMessage("PublishedDate is required when status is Scheduled")
                    .GreaterThan(DateTime.UtcNow).WithMessage("PublishedDate must be in the future")
                    .LessThan(q => q.AvailableFrom).WithMessage("PublishedDate must be before AvailableFrom");
            });

            RuleFor(q => q.MaximumAttempts)
                .GreaterThan(0).WithMessage("must be greater than 0")
                .LessThanOrEqualTo(5).WithMessage("can't exceed 5");

            RuleFor(q => q.CourseId)
                .NotNull().WithMessage("CourseId can't be null");

            RuleFor(q => q.AttemptTimeLimit)
                .NotNull().WithMessage("AttemptTimeLimit can't be null")
                .GreaterThan(0).WithMessage("must be greater than 0");
        }
    }
}

