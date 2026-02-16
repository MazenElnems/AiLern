using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.AssignmentSubmissions;
using LMS.Application.Features.AssignmentSubmissions.Queries.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQuery
    : StudentSubmissionsPaginatedQuery, IRequest<Result<PaginationResult<StudentsAssignmentSubmissionsDto>>>
{
    [BindNever]
    public int AssignmentId { get; set; }
}
