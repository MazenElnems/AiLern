using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Quizzes.Queries.GetAttemptsByQuizId;

public class GetAttemptsByQuizIdQueryHandler : IRequestHandler<GetAttemptsByQuizIdQuery, Result<GetAttemptsByQuizIdDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<GetAttemptsByQuizIdQueryHandler> _logger;

    public GetAttemptsByQuizIdQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IPermissionService permissionService, ILogger<GetAttemptsByQuizIdQueryHandler> logger)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<Result<GetAttemptsByQuizIdDto>> Handle(GetAttemptsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId);

            if (quiz == null)
                return DomainErrors.Quiz.NotFound(request.QuizId);

            var enrollmentResult = await _permissionService.AuthorizeStudentEnrollmentAsync(quiz.CourseId);
            if (!enrollmentResult.IsSuccess) return Result<GetAttemptsByQuizIdDto>.Failure(enrollmentResult.Error!);

            var attempts = await _unitOfWork.Attempts.FilterAsync(
                a => a.StudentId == user.Id && a.QuizId == request.QuizId,
                includeProperties: [nameof(Attempt.AttemptAnswers)]);

            var myAttempts = new GetAttemptsByQuizIdDto
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                attempts = attempts.Select(a =>
                {
                    var canShowScore =
                        (a.Status == AttemptStatus.Reviewed ||
                        (quiz.ShowResultOnClose && a.Status == AttemptStatus.Submitted))
                        && quiz.AvailableUntil < DateTime.UtcNow;

                    return new AttemptMetaData
                    {
                        Id = a.Id,
                        AttemptNumber = a.AttemptNumber,
                        Score = canShowScore ? a.Score : null,
                        StartAt = a.StartAt,
                        SubmittedAt = a.SubmittedAt,
                        Status = a.Status,
                        TimeSpent = a.TimeSpent
                    };
                }).ToList()
            };

            return myAttempts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching attempts for quiz {QuizId}", request.QuizId);
            throw;
        }
    }
}
