using LMS.Shared.Models;

namespace LMS.Shared.DTOs.Courses;

public class CouseQueryDto
{
    // TODO: add Validations
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
