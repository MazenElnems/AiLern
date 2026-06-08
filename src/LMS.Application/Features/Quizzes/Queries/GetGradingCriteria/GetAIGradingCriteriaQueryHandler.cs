using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetGradingCriteria;

public class GetAIGradingCriteriaQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<GetAIGradingCriteriaQuery, Result<List<AIQuizGradingCriteriaDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result<List<AIQuizGradingCriteriaDto>>> Handle(GetAIGradingCriteriaQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != currentUserId)
            return DomainErrors.Quiz.NotOwned;

        var questionCriteria = await _unitOfWork.Questions.Query
            .Where(q => q.QuizId == request.QuizId && q.Type == QuestionType.Written)
            .Select(q => new AIQuizGradingCriteriaDto
            {
                QuestionId = q.Id,
                QuestionText = q.QuestionText,
                Mark = q.Mark,
                ModelAnswer = q.AIGradingReferenceAnswer!,
                CriteriaList = q.Criterias.Select(c => new QuestionCriteriaDto
                {
                    Id = c.Id,
                    Criterion = c.Criterion,
                    Mark = c.Mark
                }).ToList()
            }).ToListAsync(cancellationToken);

        return questionCriteria;
    }
}
