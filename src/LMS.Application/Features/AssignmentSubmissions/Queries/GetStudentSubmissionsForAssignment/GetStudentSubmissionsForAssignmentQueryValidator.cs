using FluentValidation;
using LMS.Application.Common.Validators;
using LMS.Domain.Constants;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQueryValidator : BasePaginatedQueryValidator<GetStudentSubmissionsForAssignmentQuery>
{
    public GetStudentSubmissionsForAssignmentQueryValidator()
    {
        RuleFor(q => q.Status)
            .NotEmpty().WithMessage("Status must not be empty.")
            .Must(status => new List<string> { AssignmentSubmissionStatus.All, AssignmentSubmissionStatus.OnTime, AssignmentSubmissionStatus.Late}.Contains(status))
            .WithMessage("Status must be one of the following values: All, OnTime, Late.");

        RuleFor(q => q.SortBy)
            .Must(s => string.IsNullOrEmpty(s) || new List<string> { AssignmentSubmissionsSortByOptions.SubmissionDate, AssignmentSubmissionsSortByOptions.StudentName }.Contains(s))
            .WithMessage("SortBy must be one of the following values: submissiondate, studentname.");
    }
}
