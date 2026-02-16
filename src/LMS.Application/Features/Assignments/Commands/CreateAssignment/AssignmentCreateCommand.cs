using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Application.DTOs.Assignments;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.CreateAssignment;

public class AssignmentCreateCommand : IRequest<Result<AssignmentDto>>
{
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public int CourseId { get; set; }
    public bool AllowLateSubmission { get; set; }
    public bool IsPublished { get; set; }
    public List<FileMetaData>? UploadedFileMetaData { get; set; }
}
