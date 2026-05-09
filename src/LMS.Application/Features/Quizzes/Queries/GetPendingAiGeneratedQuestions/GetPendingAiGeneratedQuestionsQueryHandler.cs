using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetPendingAiGeneratedQuestions;

public class GetPendingAiGeneratedQuestionsQueryHandler
    : IRequestHandler<GetPendingAiGeneratedQuestionsQuery, Result<List<QuestionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetPendingAiGeneratedQuestionsQueryHandler(
        IUnitOfWork unitOfWork,
        IUserContext userContext,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<List<QuestionDto>>> Handle(
        GetPendingAiGeneratedQuestionsQuery request,
        CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(
            q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var questions = await _unitOfWork.Questions.Query
            .AsNoTracking()
            .Where(q => q.QuizId == request.QuizId)
            .Where(QuizQuestionVisibility.IsPendingAi)
            .Include(q => q.Options)
            .OrderBy(q => q.Order)
            .ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<QuestionDto>>(questions);
        foreach (var dto in dtos)
            dto.Options = dto.Options?.OrderBy(o => o.OptionNumber).ToList();

        return Result<List<QuestionDto>>.Success(dtos);
    }
}
