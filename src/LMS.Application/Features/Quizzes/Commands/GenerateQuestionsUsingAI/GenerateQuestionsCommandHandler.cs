using AutoMapper;
using LMS.Application.Common.Models.Request;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;
using LMS.Domain.Entities.Notification;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Quizzes.Commands.GenerateQuestionsUsingAI;

public class GenerateQuestionsCommandHandler(IUserContext userContext,
    IUnitOfWork unitOfWork,
    IAIService aiService,
    IMapper mapper,
    ILogger<GenerateQuestionsCommandHandler> logger,
    IAIServiceNotifier notifier,
    INotificationService notificationService
    ) : IRequestHandler<GenerateQuestionsCommand, Result>
{
    private readonly IUserContext _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IAIService _aiService = aiService;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<GenerateQuestionsCommandHandler> _logger = logger;
    private readonly IAIServiceNotifier _notifier = notifier;
    private readonly INotificationService _notificationService = notificationService;

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
        try
        {
            var result = await _aiService.GenerateQuestionsAsync(aiQuizGenerationRequest, cancellationToken);

            await _notificationService.NotifyAsync(
                userId,
                "Questions Generation Completed",
                $"The AI has completed generating questions for quiz '{quiz.Title}'."
            );
        }
        catch (AIServiceTimeoutException ex)
        {
            await _notifier.NotifyQuestionGenerationFailedAsync(userId, "Can not connect to AI service right now.", cancellationToken);
        }
        catch(AIServiceUnAvailableException ex)
        {
            await _notifier.NotifyQuestionGenerationFailedAsync(userId, "AI service is currently unavailable. Please try again later.", cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating quiz questions for quiz {QuizId}", request.QuizId);
            await _notifier.NotifyQuestionGenerationFailedAsync(userId, "an error occurred while generating quiz questions. Please try again later.", cancellationToken);
        }

        return Result.Success("Quiz questions generation request accepted. You will be notified once the process is completed.");
    }
}
