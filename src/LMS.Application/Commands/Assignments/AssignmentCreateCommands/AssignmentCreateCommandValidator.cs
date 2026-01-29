using FluentValidation;

namespace LMS.Application.Commands.Assignments.AssignmentCreateCommands;

public class AssignmentCreateCommandValidator : AbstractValidator<AssignmentCreateCommand>
{
    public AssignmentCreateCommandValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and must be 200 characters or less.");

        RuleFor(a => a.Instructions)
            .NotEmpty()
            .WithMessage("Instructions are required.");

        RuleFor(a => a.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be greater than 0.");

        RuleFor(a => a.DueDate)
            .Must(d => d > DateTime.UtcNow)
            .WithMessage("DueDate must be in the future.");
    }
}
