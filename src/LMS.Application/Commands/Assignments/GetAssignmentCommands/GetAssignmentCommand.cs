using LMS.Domain.DTOs.Assignments;
using MediatR;

namespace LMS.Application.Commands.Assignments.GetAssignmentCommands;

public class GetAssignmentCommand : IRequest<AssignmentWithFilesDto>
{
    public int Id { get; }
    public int CourseId { get; set; }
    public GetAssignmentCommand(int id, int courseId)
    {
        Id = id;
        CourseId = courseId;
    }
}
