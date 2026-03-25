using FluentValidation;
using LMS.Domain.Constants;

namespace LMS.Application.Features.AssignmentSubmissions.Queries.GetStudentSubmissionsForAssignment;

public class GetStudentSubmissionsForAssignmentQueryValidator : AbstractValidator<GetStudentSubmissionsForAssignmentQuery>
{
    public GetStudentSubmissionsForAssignmentQueryValidator()
    {
        RuleFor(q => q.Status)
            .NotEmpty().WithMessage("Status must not be empty.")
            .Must(status => new List<string> { AssignmentSubmissionStatus.All, AssignmentSubmissionStatus.OnTime, AssignmentSubmissionStatus.Late }.Contains(status))
            .WithMessage("Status must be one of the following values: All, OnTime, Late.");
    }
}
