using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Sections.Commands.RequestPreSignedUrl;

public class RequestMaterialPresignedUrlCommand : IRequest<Result<PreSignedUrlResponse>>
{
    public List<FileMetaData> Files { get; set; }
    [JsonIgnore]
    public Guid SectionId { get; set; }
}
