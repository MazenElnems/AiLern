using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Commands.AcceptAiGeneratedQuestion;

public class AcceptAiGeneratedQuestionCommandHandler
    : IRequestHandler<AcceptAiGeneratedQuestionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AcceptAiGeneratedQuestionCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(AcceptAiGeneratedQuestionCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var question = await _unitOfWork.Questions.Query
            .Include(q => q.Quiz)
            .ThenInclude(qz => qz.Course)
            .FirstOrDefaultAsync(
                q => q.Id == request.QuestionId && q.QuizId == request.QuizId,
                cancellationToken);

        if (question == null)
            return DomainErrors.QuizQuestion.NotFound(request.QuestionId);

        if (question.Quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        if (!question.IsAIGenerated || question.IsAccepted == true)
            return DomainErrors.QuizQuestion.NotPendingAi;

        question.IsAccepted = true;
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Question accepted.");
    }
}
