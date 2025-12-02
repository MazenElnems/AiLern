using LMS.Core.Common;
using LMS.Core.DTOs.Students;
using LMS.Domin.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Courses.GetStudentsByCourseId;

public class GetStudentsByCourseIdQuery : BasePagedQuery, IRequest<List<GetStudentsByCourseIdDto>>
{
    [JsonIgnore]
    public int Id { get; set; } 
}
