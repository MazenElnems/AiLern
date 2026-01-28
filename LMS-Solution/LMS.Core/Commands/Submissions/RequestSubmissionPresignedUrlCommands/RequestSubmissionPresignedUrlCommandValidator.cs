using FluentValidation;
using LMS.Core.Validators;

namespace LMS.Core.Commands.Submissions.RequestSubmissionPresignedUrlCommands;

public class RequestSubmissionPresignedUrlCommandValidator : AbstractValidator<RequestSubmissionPresignedUrlCommand>
{
    public RequestSubmissionPresignedUrlCommandValidator()
    {
        RuleFor(command => command.Files)
            .NotEmpty().WithMessage("At least one file must be provided.")
            .Must(files => files.Count <= 10).WithMessage("A maximum of 10 files can be requested at once.");

        RuleForEach(command => command.Files)
            .SetValidator(new FileMetaDataValidator());
    }
}
