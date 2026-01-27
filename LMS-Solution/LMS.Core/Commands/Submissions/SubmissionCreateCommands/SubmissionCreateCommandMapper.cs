using AutoMapper;
using LMS.Core.Commands.Assignments.AssignmentCreateCommands;
using LMS.Domain.Entities;

namespace LMS.Core.Commands.Submissions.SubmissionCreateCommands;

public class SubmissionCreateCommandMapper : Profile
{
    public SubmissionCreateCommandMapper()
    {
        CreateMap<SubmissionCreateCommand, AssignmentSubmission>();
    }
}
