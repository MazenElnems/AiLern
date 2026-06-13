using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Courses.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetCourseDetailsById;

public record GetCourseDetailsByIdQuery(int courseId):IRequest<Result<GetCourseDetailsDto>>;
