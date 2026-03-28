using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public DeleteQuizCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task<Result> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quizResult = await _permissionService.AuthorizeInstructorAccessToQuizAsync(request.Id);
        if (!quizResult.IsSuccess) return Result.Failure(quizResult.Error!);
        var quiz = quizResult.Value!;

        _unitOfWork.Quizzes.Delete(quiz);
        await _unitOfWork.CommitAsync();
        return Result.Success();
    }
}
