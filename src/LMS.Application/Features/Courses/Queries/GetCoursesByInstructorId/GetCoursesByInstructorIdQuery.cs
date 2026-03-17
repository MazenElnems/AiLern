using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetCoursesByInstructorId;

public record GetCoursesByInstructorIdQuery(int instructorId) : IRequest<Result<List<GetStudentCoursesDto>>>;
