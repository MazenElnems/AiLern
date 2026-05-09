using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommandValidator : AbstractValidator<AssignmentCreateCommand>
{
    public AssignmentCreateCommandValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and must be 200 characters or less.");

        RuleFor(a => a.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be greater than 0.");

        RuleFor(a => a.DueDate)
            .Must(d => d > DateTime.UtcNow)
            .WithMessage("DueDate must be in the future.");

        RuleFor(a => a.UploadedFileMetaData)
            .Must(files => files is null || files.Count <= 10)
            .WithMessage("You can upload a maximum of 10 files.");

        RuleForEach(a => a.UploadedFileMetaData)
            .SetValidator(new FileMetaDataValidator());
    }
}
