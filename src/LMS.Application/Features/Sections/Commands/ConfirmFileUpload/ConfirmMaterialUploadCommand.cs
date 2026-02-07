using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.ConfirmFileUpload;

public class ConfirmMaterialUploadCommand : IRequest<Result>
{
    public Guid SectionId { get; set; }
}
