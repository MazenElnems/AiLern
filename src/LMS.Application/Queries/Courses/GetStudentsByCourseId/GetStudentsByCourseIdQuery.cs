using LMS.Domain.Common;
using LMS.Domain.DTOs.Students;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Application.Queries.Courses.GetStudentsByCourseId;

public class GetStudentsByCourseIdQuery : BasePagedQuery, IRequest<List<GetStudentsByCourseIdDto>>
{
    [JsonIgnore]
    public int Id { get; set; } 
}
