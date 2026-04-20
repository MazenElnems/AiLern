using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmission;

public class GetSubmissionByAssignmentIdQuery : IRequest<Result<MySubmissionDto>>
{
    public int AssignmentId { get; set; }
}

