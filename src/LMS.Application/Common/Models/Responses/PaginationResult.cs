namespace LMS.Application.Common.Models.Responses;

public class PaginationResult<TValue>
{
    public int TotalResults { get; set; }
    public int PagesCount { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public List<TValue> Items { get; set; }

    public PaginationResult(int pageNumber,int pageSize,int totalResult, List<TValue> items)
    {
        TotalResults = totalResult;
        PagesCount = (int)Math.Ceiling((double)totalResult / pageSize);
        Start = ((pageNumber - 1) * pageSize) + 1;
        End = Math.Min(pageNumber * pageSize, totalResult);
        Items = items;
    }

    public static PaginationResult<TValue> CreatePaginationResult(int pageNumber, int pageSize, int totalResult, List<TValue> items)
    {
        return new PaginationResult<TValue>(pageNumber, pageSize, totalResult, items);
    }
}
