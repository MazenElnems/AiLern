using LMS.Core.Constants;
using LMS.Domin.DTOs.Courses;
using MediatR;
using System.Text.Json.Serialization;

namespace LMS.Core.Queries.Students.GetAllQuery;

public class GetStudentCoursesQuery : IRequest<List<GetStudentCoursesDto>>
{
    [JsonIgnore]
    public int Id { get; set; } 
    public string? SearchString { get; set; }
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
