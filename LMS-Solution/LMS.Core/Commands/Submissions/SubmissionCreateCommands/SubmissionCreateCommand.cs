using LMS.Domain.DTOs.Assignments;
using LMS.Domain.DTOs.Submission;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Core.Commands.Submissions.SubmissionCreateCommands;

public class SubmissionCreateCommand : IRequest<SubmissionDto>
{

    [Required]
    public int AssignmentId { get; set; }
}
