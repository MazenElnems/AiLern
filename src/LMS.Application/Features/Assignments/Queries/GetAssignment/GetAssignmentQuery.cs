using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Assignments;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetAssignment;

public class GetAssignmentQuery : IRequest<Result<AssignmentWithFilesDto>>
{
    public int Id { get; }
    public int CourseId { get; set; }
    public GetAssignmentQuery(int id, int courseId)
    {
        Id = id;
        CourseId = courseId;
    }
}
