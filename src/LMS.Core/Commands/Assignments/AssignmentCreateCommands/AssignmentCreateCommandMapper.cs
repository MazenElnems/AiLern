using AutoMapper;
using LMS.Domain.Entities;

namespace LMS.Core.Commands.Assignments.AssignmentCreateCommands;

public class AssignmentCreateCommandMapper : Profile
{
    public AssignmentCreateCommandMapper()
    {
        CreateMap<AssignmentCreateCommand, Assignment>();
    }
}
