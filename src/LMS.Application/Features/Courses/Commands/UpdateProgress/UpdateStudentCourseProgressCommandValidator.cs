using FluentValidation;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Courses.Commands.UpdateProgress;

public class UpdateStudentCourseProgressCommandValidator : AbstractValidator<UpdateStudentCourseProgressCommand>
{
    public UpdateStudentCourseProgressCommandValidator()
    {
        RuleFor(x => x.LastWatchedTime)
            .GreaterThanOrEqualTo(0)
            .When(x => x.LastWatchedTime.HasValue)
            .WithMessage("Last watched time cannot be negative.")
            .NotNull().When(x => !x.LastPageNumber.HasValue).WithMessage("Last watched time is required when last page number is not provided.");

        RuleFor(x => x.LastPageNumber)
            .GreaterThanOrEqualTo(1)
            .When(x => x.LastPageNumber.HasValue)
            .WithMessage("Last page number must be at least 1.")
            .NotNull().When(x => !x.LastWatchedTime.HasValue).WithMessage("Last page number is required when last watched time is not provided.");
    }
}
