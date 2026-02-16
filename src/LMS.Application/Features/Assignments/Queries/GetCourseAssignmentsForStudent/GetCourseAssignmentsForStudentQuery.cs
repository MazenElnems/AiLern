using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Assignments;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetCourseAssignmentsForStudent;

public class GetCourseAssignmentsForStudentQuery(int courseId) : IRequest<Result<List<GetAllAssignmentForStudentDto>>>
{
    public int CourseId { get; } = courseId;
}