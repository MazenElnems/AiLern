using AutoMapper;
using LMS.Domain.Entities;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class SubmissionCreateCommandMapper : Profile
{
    public SubmissionCreateCommandMapper()
    {
        CreateMap<SubmissionCreateCommand, AssignmentSubmission>();
    }
}
