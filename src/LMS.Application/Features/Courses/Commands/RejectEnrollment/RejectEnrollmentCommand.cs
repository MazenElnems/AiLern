using LMS.Application.Common.Results.Generic;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.RejectEnrollment;

public class RejectEnrollmentCommand(int courseId,int studentId, string reason) : IRequest<Result<string>>
{
    [JsonIgnore]
    public int CourseId { get; set; } = courseId;
    [JsonIgnore]
    public int StudentId { get; set; } = studentId;
    public string Reason { get; set; } = reason;
}
