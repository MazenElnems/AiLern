using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if(quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if(quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        // Quiz started
        if(quiz.AvailableFrom < DateTime.UtcNow)
        {
            // Can only increase AvailableUntil, MaximumAttempts, AttemptTimeLimit
            if (request.MaximumAttempts < quiz.MaximumAttempts)
                return DomainErrors.Quiz.CannotDecreaseMaximumAttempts;

            if (request.AttemptTimeLimit < quiz.AttemptTimeLimit)
                return DomainErrors.Quiz.CannotDecreaseAttemptTimeLimit;

            if (request.AvailableUntil < quiz.AvailableUntil)
                return DomainErrors.Quiz.CannotShortenQuizDuration;

            quiz.MaximumAttempts = request.MaximumAttempts;
            quiz.AvailableUntil = request.AvailableUntil;
            quiz.AttemptTimeLimit = request.AttemptTimeLimit;

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success("Quiz updated successfully");
        }
        // Not Started (can't set quiz start time at the pasts)
        else if(request.AvailableFrom < DateTime.UtcNow)
            return DomainErrors.Quiz.StartTimeCannotBeInThePast;

        quiz.Title = request.Title;
        quiz.Description = request.Description;
        quiz.MaximumAttempts = request.MaximumAttempts;
        quiz.AvailableUntil = request.AvailableUntil;
        quiz.AttemptTimeLimit = request.AttemptTimeLimit;
        quiz.AvailableFrom = request.AvailableFrom;
        quiz.ShowResultOnClose = request.ShowResultOnClose;
        quiz.ShuffleQuestions = request.ShuffleQuestions;
        quiz.ShuffleOptions = request.ShuffleOptions;

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success("Quiz updated successfully");
    }
}
