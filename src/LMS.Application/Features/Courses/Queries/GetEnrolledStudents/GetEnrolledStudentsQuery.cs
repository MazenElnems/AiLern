using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Students.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Courses.Queries.GetEnrolledStudents;

public record GetEnrolledStudentsQuery(
    int CourseId,
    int PageNo,
    int PageSize,
    string SearchString
) : IRequest<Result<PaginationResult<GetEnrolledStudentsDto>>>
{ }
