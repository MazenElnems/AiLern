using FluentValidation;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommandValidator : AbstractValidator<UpdateCourseDetailsCommand>
{
    public UpdateCourseDetailsCommandValidator()
    {
        RuleFor(c => c.Code).Length(5, 7)
            .WithMessage("Course code must be between 5 and 7 characters.");

        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image.FileSize)
                .LessThanOrEqualTo(2 * 1024 * 1024)
                .WithMessage("Image size must not exceed 2MB.");

            RuleFor(x => x.Image.ContentType)
                .Must(type => type == "image/jpeg" || type == "image/png")
                .WithMessage("Only JPEG and PNG images are allowed.");

            RuleFor(x => x.Image.FileName)
                .NotEmpty()
                .WithMessage("File name is required.");
        });
    }
}
