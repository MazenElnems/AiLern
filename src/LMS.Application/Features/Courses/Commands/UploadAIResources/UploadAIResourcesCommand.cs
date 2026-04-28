using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.UploadAIResources;

public class UploadAIResourcesCommand : IRequest<Result<List<AIResourcesDto>>>
{
    [JsonIgnore]
    public int CourseId { get; set; }
    public List<FileMetaData> Files { get; set; }
}
