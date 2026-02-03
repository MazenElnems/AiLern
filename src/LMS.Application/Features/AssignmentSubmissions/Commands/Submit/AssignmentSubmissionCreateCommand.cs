using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Submission;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class AssignmentSubmissionCreateCommand : IRequest<Result<AssignmetSubmissionDto>>
{
    public int AssignmentId { get; set; }
    public List<FileMetaData> FileMetaData { get; set; }
}
