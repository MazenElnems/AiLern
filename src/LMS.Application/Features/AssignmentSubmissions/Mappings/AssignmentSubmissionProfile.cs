using AutoMapper;
using LMS.Application.Features.AssignmentSubmissions.Commands.Submit;
using LMS.Domain.Entities;

namespace LMS.Application.Features.AssignmentSubmissions.Mappings;

public class AssignmentSubmissionProfile : Profile
{
    public AssignmentSubmissionProfile()
    {
        CreateMap<AssignmentSubmissionCreateCommand, AssignmentSubmission>();
    }
}
