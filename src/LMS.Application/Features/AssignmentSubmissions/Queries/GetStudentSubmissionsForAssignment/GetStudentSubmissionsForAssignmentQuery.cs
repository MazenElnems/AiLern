using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public record GetStudentSubmissionsForAssignmentQuery(int AssignmentId, string Status, int PageNo, int PageSize) : IRequest<Result<PaginationResult<StudentsAssignmentSubmissionsDto>>>
{ }
