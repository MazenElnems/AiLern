using FluentValidation;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommandValidator : AbstractValidator<UpdateCourseDetailsCommand>
{
    public UpdateCourseDetailsCommandValidator()
    {
        RuleFor(c => c.Code).Length(5, 7)
            .WithMessage("Course code must be between 5 and 7 characters.");
    }
}
