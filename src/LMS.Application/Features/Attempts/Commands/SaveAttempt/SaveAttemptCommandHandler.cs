using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
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

        var attemptAnswers = await _unitOfWork.AttemptAnswers.FilterAsync(a => a.AttemptId == request.AttemptId);

        var hasChanges = false;
        foreach (var answerPayload in request.Answers)
        {
            var attemptAnswer = attemptAnswers
                .LastOrDefault(a => a.QuestionId == answerPayload.QuestionId);

            if (attemptAnswer == null)
                return DomainErrors.Attempt.InvalidQuestion(answerPayload.QuestionId);

            if (attemptAnswer.BooleanAnswer != answerPayload.BooleanAnswer)
            {
                attemptAnswer.BooleanAnswer = answerPayload.BooleanAnswer;
                hasChanges = true;
            }

            if (attemptAnswer.WrittenAnswer != answerPayload.WrittenAnswer)
            {
                attemptAnswer.WrittenAnswer = answerPayload.WrittenAnswer;
                hasChanges = true;
            }

            if (attemptAnswer.OptionNumber != answerPayload.OptionNumber)
            {
                attemptAnswer.OptionNumber = answerPayload.OptionNumber;
                hasChanges = true;
            }
        }

        if (!hasChanges)
            return Result.Success("No answer changes detected.");

        attempt.SavedAt = now;
        await _unitOfWork.CommitAsync();

        return Result.Success("Answers saved successfully.");
    }
}
