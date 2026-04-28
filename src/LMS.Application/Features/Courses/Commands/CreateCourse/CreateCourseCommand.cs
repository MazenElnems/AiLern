using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommand : IRequest<Result<object>>
{
    public FileMetaData? Image { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
