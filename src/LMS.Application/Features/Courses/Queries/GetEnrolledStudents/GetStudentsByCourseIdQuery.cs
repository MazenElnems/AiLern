using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Students;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Courses.Queries.GetEnrolledStudents;

public class GetStudentsByCourseIdQuery : BasePaginatedQuery, IRequest<Result<List<GetStudentsByCourseIdDto>>>
{
    [JsonIgnore]
    public int Id { get; set; } 
}
