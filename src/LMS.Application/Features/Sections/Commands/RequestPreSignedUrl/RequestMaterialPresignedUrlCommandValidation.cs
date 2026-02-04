using FluentValidation;
using LMS.Application.Features.Assignments.Commands.PresignedUrlFileUpload;
using LMS.Application.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl
{
    public class RequestMaterialPresignedUrlCommandValidation : AbstractValidator<RequestMaterialPresignedUrlCommand>
    {
        public RequestMaterialPresignedUrlCommandValidation()
        {
            RuleFor(command => command.Files)
                .NotEmpty().WithMessage("At least one file must be provided.")
                .Must(files => files.Count <= 10).WithMessage("A maximum of 10 files can be requested at once.");

            RuleForEach(command => command.Files)
                .SetValidator(new MaterialFileMetaDataValidator());
        }

    }
}
