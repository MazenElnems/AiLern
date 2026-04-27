using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseCommandValidator()
        {

            RuleFor(c => c.Code)
            .NotEmpty()
            .WithMessage("Course code is required.")
            .Length(5, 7)
            .WithMessage("Course code must be between 5 and 7 characters.")
            .Matches("^[a-zA-Z0-9]+$")
            .WithMessage("Course code must contain only letters and numbers.")
            .Must(code => code == code.Trim())
            .WithMessage("Course code must not contain leading or trailing spaces.");


            RuleFor(c => c.Name)
                .NotEmpty()
                .WithMessage("Course name is required.")
                .MinimumLength(3)
                .WithMessage("Course name must be at least 3 characters.")
                .MaximumLength(200)
                .WithMessage("Course name must not exceed 200 characters.");


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
}
