using LMS.Domin.Constants;
using LMS.Domin.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetPendingQueries;

public class GetCoursesByStatusQuery : IRequest<List<GetCourseDto>>
{
    public string Status { get; set; }
    public string? SearchString { get; set; }
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
