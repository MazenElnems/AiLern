using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs;
using LMS.Domain.DTOs.Assignments;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Assignments.Commands.UpdateAssignment;

public class AssignmentUpdateCommand : IRequest<Result<AssignmentDto>>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public DateTime DueDate { get; set; }
    public bool AllowLateSubmission { get; set; }
    public bool IsPublished { get; set; }
    public List<FileMetaData> UploadedFileMetaData { get; set; } 
}
