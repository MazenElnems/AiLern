using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public DeleteQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Quiz.Course)]);
        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.Id);
        if (quiz.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;
        _unitOfWork.Quizzes.Delete(quiz);
        await _unitOfWork.CommitAsync();
        return Result.Success();

    }
}
