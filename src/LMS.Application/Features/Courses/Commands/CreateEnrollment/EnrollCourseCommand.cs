using LMS.Application.Common.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.CreateEnrollment;

public class EnrollCourseCommand(int courseId ,string studentEmail) : IRequest<Result>
{
    [JsonIgnore]
    public int CourseId { get; set; } = courseId;
    public string StudentEmail { get; set; } = studentEmail;
}
