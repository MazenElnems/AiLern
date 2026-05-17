using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.ProcessAIDocs;

public record AIServiceProcessDocsCommand(int CourseId, List<Guid> DocumentIds) : IRequest<Result>;