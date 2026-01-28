namespace LMS.Domain.DTOs;

public class PaginationResult<T>
{
    public int TotalResults { get; set; }
    public int PagesCount { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public List<T> Items { get; set; }

    public PaginationResult(int pageNumber,int pageSize,int totalResult, List<T> items)
    {
        TotalResults = totalResult;
        PagesCount = (int)Math.Ceiling((double)totalResult / pageSize);
        Start = ((pageNumber - 1) * pageSize) + 1;
        End = Math.Min(pageNumber * pageSize, totalResult);
        Items = items;
    }
}
