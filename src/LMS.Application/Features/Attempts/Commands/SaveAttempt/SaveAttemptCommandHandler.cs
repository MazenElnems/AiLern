using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SaveAttempt;

public class SaveAttemptCommandHandler : IRequestHandler<SaveAttemptCommand, Result>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public SaveAttemptCommandHandler(IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(SaveAttemptCommand request, CancellationToken cancellationToken)
    {
        var attemptResult = await _permissionService.AuthorizeStudentAccessToAttemptAsync(request.AttemptId);
        if (!attemptResult.IsSuccess) return Result.Failure(attemptResult.Error!);
        var attempt = attemptResult.Value!;

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
