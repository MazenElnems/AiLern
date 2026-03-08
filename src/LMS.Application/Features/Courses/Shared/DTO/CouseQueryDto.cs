using LMS.Domain.Constants;

namespace LMS.Application.Features.Courses.Shared.DTO;

public class CouseQueryDto
{
    // TODO: add Validations
    public string? SortBy { get; set; }
    public string? Order { get; set; } = SortOrderOptions.DESC;
    public string? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
