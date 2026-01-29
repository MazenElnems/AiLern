using AutoMapper;
using LMS.Domain.Entities;

namespace LMS.Application.Commands.Assignments.AssignmentCreateCommands;

public class AssignmentCreateCommandMapper : Profile
{
    public AssignmentCreateCommandMapper()
    {
        CreateMap<AssignmentCreateCommand, Assignment>();
    }
}
