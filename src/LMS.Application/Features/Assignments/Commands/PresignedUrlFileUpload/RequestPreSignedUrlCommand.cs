using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.PresignedUrlFileUpload;

public class RequestPreSignedUrlCommand : IRequest<Result<PreSignedUrlResponse>>
{
    public List<FileMetaData> Files { get; set; }
    public int AssignmentId { get; set; }
}
