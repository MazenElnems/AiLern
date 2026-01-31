using LMS.Application.Common.Results.Generic;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommand : IRequest<Result<int>>
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
