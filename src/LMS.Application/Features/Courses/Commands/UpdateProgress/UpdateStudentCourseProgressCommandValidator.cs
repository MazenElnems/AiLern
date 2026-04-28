using FluentValidation;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Commands.UpdateProgress;

public class UpdateStudentCourseProgressCommandValidator : AbstractValidator<UpdateStudentCourseProgressCommand>
{
    public UpdateStudentCourseProgressCommandValidator()
    {
        RuleFor(x => x.CompletedSections)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Completed sections cannot be negative.");

        RuleFor(x => x.LastWatchedTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LastWatchedTime.HasValue)
            .WithMessage("Last watched time cannot be negative.");

        RuleFor(x => x.LastPageNumber)
            .GreaterThanOrEqualTo(1)
            .When(x => x.LastPageNumber.HasValue)
            .WithMessage("Last page number must be at least 1.");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Learning type is not valid.");
    }
}
