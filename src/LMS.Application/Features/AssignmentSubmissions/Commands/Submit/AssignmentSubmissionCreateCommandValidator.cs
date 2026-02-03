using FluentValidation;
using LMS.Application.Validators;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit
{
    public class AssignmentSubmissionCreateCommandValidator : AbstractValidator<AssignmentSubmissionCreateCommand>
    {
        public AssignmentSubmissionCreateCommandValidator()
        {
            RuleFor(command => command.FileMetaData)
                .NotEmpty().WithMessage("At least one file must be provided.")
                .Must(files => files.Count <= 10).WithMessage("A maximum of 10 files can be requested at once.");

            RuleForEach(command => command.FileMetaData)
                .SetValidator(new FileMetaDataValidator());
        }   
    }
}

