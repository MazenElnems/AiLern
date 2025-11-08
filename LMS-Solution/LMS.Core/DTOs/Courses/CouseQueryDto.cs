using LMS.Core.Constants;

namespace LMS.Core.DTOs.Courses;

public class CouseQueryDto
{
    // TODO: add Validations
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
