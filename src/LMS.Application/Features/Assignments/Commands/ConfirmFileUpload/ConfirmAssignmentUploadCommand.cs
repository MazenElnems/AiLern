using LMS.Application.Common.Results;
using MediatR;

namespace LMS.Application.Features.Assignments.Commands.ConfirmFileUpload;

public class ConfirmAssignmentUploadCommand : IRequest<Result>
{
    public int AssignmentId { get; set; }
}
