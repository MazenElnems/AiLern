using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetAIResources;

public class GetAIResourcesCommand : IRequest<Result<List<AiFileDto>>>
{
    public int CourseId { get; set; }
}
