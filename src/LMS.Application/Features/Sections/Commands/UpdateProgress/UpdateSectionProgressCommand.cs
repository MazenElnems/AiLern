using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.UpdateProgress;

public record UpdateSectionProgressCommand(
    Guid SectionId,
    bool IsCompleted
    ) : IRequest<Result>
{

}
