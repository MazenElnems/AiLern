using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.AssignmentSubmissions;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class AssignmentSubmissionCreateCommand : IRequest<Result<AssignmentSubmissionDto>>
{
    public int AssignmentId { get; set; }
    public List<FileMetaData> FileMetaData { get; set; }
}
