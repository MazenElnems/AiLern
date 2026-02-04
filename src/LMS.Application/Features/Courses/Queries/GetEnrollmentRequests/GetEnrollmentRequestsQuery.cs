using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.DTOs.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Application.Features.Courses.Queries.GetEnrollmentRequests;

public class GetEnrollmentRequestsQuery : BasePaginatedQuery, IRequest<Result<List<GetEnrollmentRequestsDto>>>
{
    [BindNever]
    public int CourseId { get; set; }
}
