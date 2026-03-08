using FluentValidation;
using LMS.Application.Features.Quizzes.Shared.Requests;

namespace LMS.Application.Features.Quizzes.Shared.Validators;

public class OptionRequestValidator : AbstractValidator<OptionRequest>
{
    public OptionRequestValidator()
    {
        RuleFor(o => o.OptionText)
            .NotNull().WithMessage("Option text cannot be null.")
            .NotEmpty().WithMessage("Option text is required.")
            .MaximumLength(500).WithMessage("Option text cannot exceed 500 characters.");

        RuleFor(o => o.IsCorrect)
            .NotNull().WithMessage("IsCorrect field is required.");
    }
}

