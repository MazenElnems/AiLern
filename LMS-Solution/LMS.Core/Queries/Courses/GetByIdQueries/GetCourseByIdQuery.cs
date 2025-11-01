using LMS.Shared.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetByIdQueries;

public class GetCourseByIdQuery(int id) : IRequest<GetCourseDto>
{
    public int Id { get; } = id;
}
