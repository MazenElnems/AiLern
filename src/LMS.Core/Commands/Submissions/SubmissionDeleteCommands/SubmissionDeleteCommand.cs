using MediatR;

namespace LMS.Core.Commands.Submissions.SubmissionDeleteCommands;

public sealed record SubmissionDeleteCommand(int Id) : IRequest
{

}
