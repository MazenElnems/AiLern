using LMS.Domin.Common;
using LMS.Domin.DTOs.Students;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Courses.GetStudentsByCourseId;

public class GetStudentsByCourseIdQuery : BasePagedQuery, IRequest<List<GetStudentsByCourseIdDto>>
{
    [JsonIgnore]
    public int Id { get; set; } 
}
