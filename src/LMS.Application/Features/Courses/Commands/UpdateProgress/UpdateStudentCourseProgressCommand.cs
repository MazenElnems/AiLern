using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Enums;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.UpdateProgress;

public record UpdateStudentCourseProgressCommand(
    int? LastWatchedTime,
    int? LastPageNumber,
    Guid? LastOpenedFileId
    ) : IRequest<Result>
{
    [JsonIgnore]
    public int CourseId { get; set; }
}
