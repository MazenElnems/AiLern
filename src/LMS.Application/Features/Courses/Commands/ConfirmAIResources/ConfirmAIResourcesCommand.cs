using LMS.Application.Common.Results.Generic;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.ConfirmAIResources;

public class ConfirmAIResourcesCommand : IRequest<Result<List<string>>>
{
    [JsonIgnore]
    public int CourseId { get; set; }
    public List<Guid> AiResourceIds { get; set; }
}
