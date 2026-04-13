using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
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

        if(attempt == null) 
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if(attempt.StudentId != user.Id)
            return DomainErrors.Attempt.NotOwned;

        if(DateTime.UtcNow > attempt.AttemptEndTime)
            return DomainErrors.Attempt.TimeExpired;

        var answers = request.Answers.Select(a => new Answer
        {
            AttemptId = attempt.Id,
            QuestionId = a.QuestionId,
            WrittenAnswer = a.WrittenAnswer,
            OptionId = a.OptionId
        }).ToArray();

        _unitOfWork.Answers.UpdateRange(answers);
        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success();
    }
}

