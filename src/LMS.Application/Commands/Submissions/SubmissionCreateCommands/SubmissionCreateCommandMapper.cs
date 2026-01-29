using AutoMapper;
using LMS.Application.Commands.Assignments.AssignmentCreateCommands;
using LMS.Domain.Entities;

namespace LMS.Application.Commands.Submissions.SubmissionCreateCommands;

public class SubmissionCreateCommandMapper : Profile
{
    public SubmissionCreateCommandMapper()
    {
        CreateMap<SubmissionCreateCommand, AssignmentSubmission>();
    }
}
