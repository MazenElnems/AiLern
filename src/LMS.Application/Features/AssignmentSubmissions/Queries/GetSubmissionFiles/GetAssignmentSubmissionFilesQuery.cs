using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetSubmissionFiles;

public class GetAssignmentSubmissionFilesQuery(int assignmentId, int submissionId) : IRequest<Result<List<SubmissionFilesDto>>>
{
    public int AssignmentId { get; set; } = assignmentId;
    public int SubmissionId { get; set; } = submissionId;   
}
