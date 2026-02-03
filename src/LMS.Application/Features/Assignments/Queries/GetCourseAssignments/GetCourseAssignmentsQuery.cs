using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Assignments;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignments;

public class GetCourseAssignmentsQuery(int courseId) : IRequest<Result<List<GetAllAssignmentDto>>>
{
    public int CourseId { get; set; } = courseId;   
}
