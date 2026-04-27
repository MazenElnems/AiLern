using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Courses.Commands.DeleteAIResources;

public class DeleteAIResourcesCommand : IRequest<Result>
{
    public int CourseId { get; set; }
    public Guid AiResourceId { get; set; }
}
