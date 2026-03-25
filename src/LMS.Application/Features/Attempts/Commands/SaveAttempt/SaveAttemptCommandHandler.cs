using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SaveAttempt;

public class SaveAttemptCommandHandler : IRequestHandler<SaveAttemptCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public SaveAttemptCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SaveAttemptCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var attempt = await _unitOfWork.Attempts.GetByIdAsync(request.AttemptId);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != user.Id)
            return DomainErrors.Common.Forbidden("You are not allowed to access this attempt.");

        if (attempt.Status != AttemptStatus.InProgress)
            return DomainErrors.Attempt.NotInProgress;

        var now = DateTime.UtcNow;
        if (now > attempt.AttemptEndTime)
            return DomainErrors.Attempt.TimeExpired;

        if (request.Answers is null || request.Answers.Count == 0)
            return Result.Success("No answer changes detected.");

        var attemptAnswers = request.Answers
            .Select(a => new AttemptAnswer
            {
                AttemptId = request.AttemptId,
                BooleanAnswer = a.BooleanAnswer,
                OptionNumber = a.OptionNumber,
                WrittenAnswer = a.WrittenAnswer,
                QuestionId = a.QuestionId,
            }).ToArray();

        attempt.SavedAt = now;
        _unitOfWork.AttemptAnswers.UpdateRange(attemptAnswers);

        await _unitOfWork.CommitAsync();

        return Result.Success("Answers saved successfully.");
    }
}
