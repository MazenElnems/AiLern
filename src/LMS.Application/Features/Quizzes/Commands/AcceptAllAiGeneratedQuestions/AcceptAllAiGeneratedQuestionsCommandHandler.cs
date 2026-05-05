using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Commands.AcceptAllAiGeneratedQuestions;

public class AcceptAllAiGeneratedQuestionsCommandHandler
    : IRequestHandler<AcceptAllAiGeneratedQuestionsCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AcceptAllAiGeneratedQuestionsCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(AcceptAllAiGeneratedQuestionsCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var pending = await _unitOfWork.Questions.TrackedQuery
            .Where(q => q.QuizId == request.QuizId)
            .Where(QuizQuestionVisibility.IsPendingAi)
            .ToListAsync(cancellationToken);

        foreach (var q in pending)
            q.IsAccepted = true;

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("All pending AI questions accepted.");
    }
}
