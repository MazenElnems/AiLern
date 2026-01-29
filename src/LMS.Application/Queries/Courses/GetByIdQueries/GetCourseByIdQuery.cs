using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Queries.Courses.GetByIdQueries;

public class GetCourseByIdQuery(int id) : IRequest<GetCourseDto>
{
    public int Id { get; } = id;
}
