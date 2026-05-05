using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Quizzes.Commands.AddBatchQuestions;

public class AddBatchQuestionsByTypeCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ILogger<AddBatchQuestionsByTypeCommandHandler> logger, IAIServiceNotifier aiServiceNotifier)
    : IRequestHandler<AddBatchQuestionsByTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AddBatchQuestionsByTypeCommandHandler> _logger = logger;
    private readonly IAIServiceNotifier _aiServiceNotifier = aiServiceNotifier;

    public async Task<Result> Handle(AddBatchQuestionsByTypeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("New batch of questions being added from ai service for Quiz with ID: {QuizId}", request.QuizId);

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
        {
            _logger.LogWarning("Quiz with ID {QuizId} not found.", request.QuizId);
            return DomainErrors.Quiz.NotFound(request.QuizId);
        }

        quiz.Questions.AddRange(_mapper.Map<List<Question>>(request.Questions));
        await _unitOfWork.CommitAsync(cancellationToken);

        var instructorId = quiz.Course.InstructorId;
        await _aiServiceNotifier.NotifyQuestionGeneratedAsync(instructorId, request.GeneratedQuestions, request.Completed, cancellationToken);
        return Result.Success();
    }
}
