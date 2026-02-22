using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Assignments.DTO;
using MediatR;

namespace LMS.Application.Features.Assignments.Queries.GetAssignment;

public class GetAssignmentQuery(int Id) : IRequest<Result<AssignmentWithFilesDto>>
{
    public int Id { get; } = Id;
}
