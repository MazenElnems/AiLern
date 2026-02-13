using FluentValidation;
using LMS.Application.Validators;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;

public class RequestMaterialPresignedUrlCommandValidation : AbstractValidator<RequestMaterialPresignedUrlCommand>
{
    public RequestMaterialPresignedUrlCommandValidation()
    {
        RuleFor(command => command.Files)
            .NotEmpty().WithMessage("At least one file must be provided.")
            .Must(files => files.Count <= 10).WithMessage("A maximum of 10 files can be requested at once.");

        RuleForEach(command => command.Files)
            .SetValidator(new FileMetaDataValidator());
    }

}
