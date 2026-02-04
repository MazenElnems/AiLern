using LMS.Application.Common.Models.Request;
using LMS.Domain.Constants;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.Responses;

public class StudentSubmissionsPaginatedQuery : BasePaginatedQuery
{
    public string? SearchString { get; set; }
    public string Status { get; set; } = AssignmentSubmissionStatus.All;
}
