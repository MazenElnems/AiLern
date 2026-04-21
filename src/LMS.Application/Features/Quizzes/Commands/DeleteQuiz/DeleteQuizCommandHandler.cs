using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using LMS.Domain.Entities.Quizzes;
using MediatR;
using LMS.Application.Contracts.UnitOfWork;

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
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.Id);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        if(quiz.AvailableFrom < DateTime.UtcNow)
            return DomainErrors.Quiz.CannotDeleteQuizDuration;

        _unitOfWork.Quizzes.Delete(quiz);

        await _unitOfWork.CommitAsync();

        return Result.Success();
    }
}
