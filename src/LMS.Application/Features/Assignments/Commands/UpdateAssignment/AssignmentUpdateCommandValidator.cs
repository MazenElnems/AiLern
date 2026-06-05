using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommandValidator : AbstractValidator<AssignmentUpdateCommand>
{
    public AssignmentUpdateCommandValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and must be 200 characters or less.");

        RuleFor(a => a.Instructions)
            .NotEmpty()
            .WithMessage("Instructions are required.");

        RuleFor(a => a.UploadedFileMetaData)
            .Must(files => files is null || files.Count <= 10)
            .WithMessage("You can upload a maximum of 10 files.");

        RuleForEach(a => a.UploadedFileMetaData)
            .SetValidator(new FileMetaDataValidator());
    }
}
