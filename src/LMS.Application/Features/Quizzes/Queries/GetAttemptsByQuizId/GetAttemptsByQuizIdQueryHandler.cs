using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Quizzes.Queries.GetAttemptsByQuizId;

public class GetAttemptsByQuizIdQueryHandler : IRequestHandler<GetAttemptsByQuizIdQuery, Result<List<GetAttemptsByQuizIdDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _uintOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAttemptsByQuizIdQueryHandler> _logger;


    public GetAttemptsByQuizIdQueryHandler(IUserContext userContext, IUnitOfWork uintOfWork, IMapper mapper, ILogger<GetAttemptsByQuizIdQueryHandler> logger)
    {
        _userContext = userContext;
        _uintOfWork = uintOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<List<GetAttemptsByQuizIdDto>>> Handle(GetAttemptsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var quiz = await _uintOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId);

            if (quiz == null)
                return DomainErrors.Quiz.NotFound(request.QuizId);

            var isEnrolled = await _uintOfWork.Enrollments.IsEnrolledAsync(quiz.CourseId, user.Id);

            if (!isEnrolled)
                return DomainErrors.Course.NotEnrolled;

            var attempts = await _uintOfWork.Attempts.FilterAsync(a => a.StudentId == user.Id && a.QuizId == request.QuizId, includeProperties: [nameof(Attempt.AttemptAnswers)]);

            var myAttempts = attempts.Select(a =>
            {

                var canShowScore = quiz.ShowResultOnClose && quiz.AvailableUntil < DateTime.UtcNow && a.Status == AttemptStatus.Reviewed;

                return new GetAttemptsByQuizIdDto
                {
                    Id = a.Id,
                    QuizId = quiz.Id,
                    QuizTitle = quiz.Title,
                    AttemptNumber = a.AttemptNumber,
                    Score = canShowScore ? a.Score : null,
                    StartAt = a.StartAt,
                    SubmittedAt = a.SubmittedAt,
                    Status = a.Status,
                    TimeSpent = a.TimeSpent
                };

            }).ToList();

            return myAttempts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching attempts for quiz {QuizId}", request.QuizId);

            throw;
        }
    }
}
