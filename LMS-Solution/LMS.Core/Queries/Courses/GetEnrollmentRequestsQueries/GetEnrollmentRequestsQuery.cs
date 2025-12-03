using LMS.Core.Common;
using LMS.Domin.DTOs.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LMS.Core.Queries.Courses.GetEnrollmentRequestsQueries;

public class GetEnrollmentRequestsQuery : BasePagedQuery, IRequest<List<GetEnrollmentRequestsDto>>
{
    [BindNever]
    public int CourseId { get; set; }
}
