using LMS.Domain.Constants;

namespace LMS.Application.Common.Models.Request;

public abstract class BasePaginatedQuery 
{
    public string? SortBy { get; set; }
    public string Order { get; set; } = SortOrderOptions.DESC;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
