using AutoMapper;
using LMS.Application.Features.AssignmentSubmissions.Commands.Submit;
using LMS.Application.Features.AssignmentSubmissions.Shared.DTO;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Features.AssignmentSubmissions.Shared.Mappings;

public class AssignmentSubmissionProfile : Profile
{
    public AssignmentSubmissionProfile()
    {
        CreateMap<AssignmentSubmissionCreateCommand, AssignmentSubmission>();
        CreateMap<AssignmentSubmission, StudentsAssignmentSubmissionsDto>()
            .ForMember(dto => dto.Name, src => src.MapFrom(src => src.Student.FullName))
            .ForMember(dto => dto.Email, src => src.MapFrom(src => src.Student.Email));
    }
}

