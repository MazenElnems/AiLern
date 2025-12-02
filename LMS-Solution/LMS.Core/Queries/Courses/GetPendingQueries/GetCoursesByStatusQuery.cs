using LMS.Core.Constants;
using LMS.Core.DTOs.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

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
