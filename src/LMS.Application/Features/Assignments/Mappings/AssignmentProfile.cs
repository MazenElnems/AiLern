using AutoMapper;
using LMS.Application.Features.Assignments.Commands.CreateAssignment;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.Entities;

namespace LMS.Application.Features.Assignments.Mappings;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<AssignmentCreateCommand, Assignment>();
        CreateMap<Assignment, AssignmentDto>();
        CreateMap<Assignment, GetAllAssignmentDto>();
    }
}
