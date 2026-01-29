using LMS.Domain.DTOs.Assignments;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Commands.Assignments.AssignmentCreateCommands;

public class AssignmentCreateCommand : IRequest<AssignmentDto>
{
    [Required]
    public string Title { get; set; }
    [Required]
    public string Instructions { get; set; }
    [Required]
    public DateTime DueDate { get; set; }
    [Required]
    public int CourseId { get; set; }
    [Required]
    public bool AllowLateSubmission { get; set; }
}
