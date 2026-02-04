using LMS.Application.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LMS.Application.Features.Sections.Commands.DeleteSection;

public sealed record SectionDeleteCommand(Guid Id) : IRequest<Result>
{
}
