using AutoMapper;
using LMS.Application.Features.Assignments.Commands.CreateAssignment;
using LMS.Application.Features.Assignments.Commands.UpdateAssignment;
using LMS.Application.Features.Assignments.DTO;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Features.Assignments.Mappings;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<AssignmentCreateCommand, Assignment>();
        CreateMap<AssignmentUpdateCommand, Assignment>();
        CreateMap<Assignment, AssignmentDto>();
        CreateMap<Assignment, GetAllAssignmentForInstructorDto>();
        CreateMap<Assignment, GetAllAssignmentForStudentDto>()
            .ForMember(dto => dto.IsSubmitted, opt => opt.MapFrom(src => src.Submissions.Any()))
            .ForMember(dto => dto.SubmissionDate, opt => opt.MapFrom(src => src.Submissions.Any() ? src.Submissions.First().SubmissionDate : default));
    }
}
