using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Assignments;
using LMS.Domain.DTOs.Submission;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.Submit;

public class SubmissionCreateCommand : IRequest<Result<SubmissionDto>>
{

    [Required]
    public int AssignmentId { get; set; }
}
