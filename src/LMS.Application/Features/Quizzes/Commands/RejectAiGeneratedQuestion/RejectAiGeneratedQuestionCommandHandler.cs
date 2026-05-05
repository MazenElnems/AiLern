using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.RejectAiGeneratedQuestion;

public class RejectAiGeneratedQuestionCommandHandler
    : IRequestHandler<RejectAiGeneratedQuestionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public RejectAiGeneratedQuestionCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(RejectAiGeneratedQuestionCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var question = await _unitOfWork.Questions.GetAsync(
            q => q.Id == request.QuestionId && q.QuizId == request.QuizId);

        if (question == null)
            return DomainErrors.QuizQuestion.NotFound(request.QuestionId);

        if (!question.IsAIGenerated || question.IsAccepted == true)
            return DomainErrors.QuizQuestion.NotPendingAi;

        if (await _unitOfWork.Answers.AnyAsync(a => a.QuestionId == question.Id, cancellationToken))
            return DomainErrors.QuizQuestion.CannotRejectHasAttempts;

        _unitOfWork.Questions.Delete(question);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Question rejected and removed.");
    }
}
