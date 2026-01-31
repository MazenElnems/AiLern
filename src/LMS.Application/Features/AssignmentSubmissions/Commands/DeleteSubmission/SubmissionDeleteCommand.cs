using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.DeleteSubmission;

public sealed record SubmissionDeleteCommand(int Id) : IRequest<Result>
{

}
