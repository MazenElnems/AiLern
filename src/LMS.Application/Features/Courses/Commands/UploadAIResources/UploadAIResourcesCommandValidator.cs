using FluentValidation;
using LMS.Application.Common.Validators;

namespace LMS.Application.Features.Courses.Commands.UploadAIResources;

public class UploadAIResourcesCommandValidator : AbstractValidator<UploadAIResourcesCommand>
{
    public UploadAIResourcesCommandValidator()
    {


        RuleFor(x => x.Files)
            .NotNull()
            .WithMessage("Files list cannot be null.")
            .NotEmpty()
            .WithMessage("At least one file must be uploaded.")
            .Must(files => files.Count <= 10)
            .WithMessage("You cannot upload more than 10 files at once.");

        RuleForEach(x => x.Files)
            .SetValidator(new AiFileMetaDataValidator());
    }
}
