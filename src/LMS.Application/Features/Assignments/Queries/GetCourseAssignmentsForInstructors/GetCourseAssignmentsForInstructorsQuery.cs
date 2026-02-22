using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.DTO;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForInstructors;

public class GetCourseAssignmentsForInstructorsQuery(int courseId) : IRequest<Result<List<GetAllAssignmentForInstructorDto>>>
{
    public int CourseId { get; set; } = courseId;   
}
