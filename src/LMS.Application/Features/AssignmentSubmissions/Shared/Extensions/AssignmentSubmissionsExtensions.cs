using LMS.Domain.Constants;
using LMS.Domain.Entities.Assignments;

namespace LMS.Application.Features.AssignmentSubmissions.Shared.Extensions;

public static class AssignmentSubmissionsExtensions
{
    public static IQueryable<AssignmentSubmission> ApplayAssignmentSubmissionStatusFilter(
        this IQueryable<AssignmentSubmission> query,
        string status
    )
    {
        if(string.IsNullOrWhiteSpace(status))
        {
            return query;
        }

        return status.ToLower() switch
        {
            AssignmentSubmissionStatus.OnTime => query.Where(submission => !submission.IsLate),
            AssignmentSubmissionStatus.Late => query.Where(submission => submission.IsLate),
            AssignmentSubmissionStatus.All => query,
            _ => query
        };
    }

    public static IQueryable<AssignmentSubmission> ApplayAssignmentSubmissionSearchFilter(
        this IQueryable<AssignmentSubmission> query,
        string searchTerm
    )
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        searchTerm = searchTerm.ToLower();

        return query.Where(submission =>
            submission.Student.FullName.ToLower().Contains(searchTerm) ||
            submission.Student.Email!.ToLower().Contains(searchTerm)
        );
    }
}