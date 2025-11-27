using LMS.Core.Constants;
using LMS.Core.DTOs.Courses;
using MediatR;

namespace LMS.Core.Queries.Courses.GetAllQueries;

public class GetAllCoursesQuery : IRequest<List<GetAllCoursesDto>>
{
    public string? SearchString { get; set; }
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
