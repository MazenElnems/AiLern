using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;

public class RequestMaterialPresignedUrlCommand : IRequest<Result<List<string>>>
{
    public List<FileMetaData> Files { get; set; }
    [JsonIgnore]
    public Guid SectionId { get; set; }
}
