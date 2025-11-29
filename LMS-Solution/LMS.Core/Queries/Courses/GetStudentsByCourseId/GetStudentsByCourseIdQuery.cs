using LMS.Domin.Entities;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Courses.GetStudentsByCourseId
{
    public class GetStudentsByCourseIdQuery(int id) : IRequest<List<string>>
    {
        [JsonIgnore]
        public int Id { get; set; } = id;
    }
}
