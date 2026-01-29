using MediatR;

namespace LMS.Application.Commands.Submissions.SubmissionDeleteCommands;

public sealed record SubmissionDeleteCommand(int Id) : IRequest
{

}
