using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Sections.Commands.DeleteSection;

public sealed record SectionDeleteCommand(Guid Id) : IRequest<Result>
{
}
