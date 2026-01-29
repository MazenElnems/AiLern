using LMS.Domain.DTOs;
using MediatR;

namespace LMS.Application.Commands.Assignments.RequestPreSignedUrlCommands;

public class RequestPreSignedUrlCommand : IRequest<PreSignedUrlResponse>
{
    public List<FileMetaData> Files { get; set; }
    public int AssignmentId { get; set; }
}
