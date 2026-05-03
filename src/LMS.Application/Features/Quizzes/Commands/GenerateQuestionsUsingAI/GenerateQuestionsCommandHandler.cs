using AutoMapper;
using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Commands.GenerateQuestionsUsingAI;

public class GenerateQuestionsCommandHandler(IUserContext userContext, IUnitOfWork unitOfWork, IAIService aiService, IMapper mapper)
    : IRequestHandler<GenerateQuestionsCommand, Result>
{
    private readonly IUserContext _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAIService _aiService = aiService;
    private readonly IMapper _mapper = mapper;

    public async Task<Result> Handle(GenerateQuestionsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz is null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != userId)
            return DomainErrors.Quiz.NotOwned;

        var aiResources = await _unitOfWork.AIResources.Query
            .Where(r => r.CourseId == quiz.CourseId && r.AIStatus == AIStatus.Completed)
            .Select(r => r.Id)
            .ToListAsync(cancellationToken);

        foreach (var fileId in request.FileIds)
        {
            if (!aiResources.Contains(fileId))
                return DomainErrors.AiResource.NotFound(fileId);
        }

        var aiQuizGenerationRequest = _mapper.Map<AIQuizGenerationRequest>(request);
        var result = await _aiService.GenerateQuestionsAsync(aiQuizGenerationRequest, cancellationToken);

        return Result.Success(result.Message);
    }
}
