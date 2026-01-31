using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Courses;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetById;

public class GetCourseByIdQuery(int id) : IRequest<Result<GetCourseDto>>
{
    public int Id { get; } = id;
}
