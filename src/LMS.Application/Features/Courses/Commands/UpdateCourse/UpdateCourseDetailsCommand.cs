using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseDetailsCommand : IRequest<Result<string>>
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public FileMetaData? Image { get; set; }

}
