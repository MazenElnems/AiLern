using LMS.Core.Constants;
using LMS.Core.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetPendingQueries;

public class GetPendingCoursesQuery : IRequest<List<GetCourseDto>>
{
    public string? SearchString { get; set; }
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
