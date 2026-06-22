using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Attempts.Commands.EvaluateAiGeneratedQuestionGrading;

public class EvaluateAiQuestionGradingCommandHandler : IRequestHandler<EvaluateAiQuestionGradingCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public EvaluateAiQuestionGradingCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(EvaluateAiQuestionGradingCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.Query.AsNoTracking().Where(a =>a.Id == request.AttemptId).FirstOrDefaultAsync();
        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (!attempt.IsAIGraded)
            return DomainErrors.Attempt.NotGradedByAi;

        var answer = await _unitOfWork.Answers.Query
            .Where(a=>a.AttemptId==request.AttemptId && a.QuestionId==request.QuestionId)
            .FirstOrDefaultAsync();

        if (answer == null)
            return DomainErrors.Common.NotFound("Answer", $"{{{request.AttemptId}}},{{{request.QuestionId}}}");

        if (request.FeedbackThemes == Domain.Enums.FeedbackThemes.Other && request.EvaluateComment == null)
            return DomainErrors.Common.BusinessRule("Evaluate Comment", "Evaluate Comment is required when you select other.");

        answer.AccuracyRating = request.AccuracyRating;
        answer.FeedbackThemes = request.FeedbackThemes;
        answer.EvaluateComment = request.EvaluateComment;

        await _unitOfWork.CommitAsync();

        return Result.Success("Ai Grading is Evaluated");
    }
}
