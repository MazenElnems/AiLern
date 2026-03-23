using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly ILogger<GradeSubmissionCommandHandler> _logger;

    public GradeSubmissionCommandHandler(IUnitOfWork unitOfWork, IUserContext user, ILogger<GradeSubmissionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _user = user;
        _logger = logger;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _user.GetCurrentUser().Id;
            var attempt = await _unitOfWork.Attempts.GetAsync(a => a.Id == request.Id, includeProperties: [nameof(Attempt.AttemptAnswers)]);
            if (attempt == null)
            {
                return DomainErrors.Attempt.NotFound(request.Id);
            }
            var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == attempt.QuizId, includeProperties: [nameof(Quiz.Course)]);
            if (quiz.Course.InstructorId != userId)
            {
                return DomainErrors.Course.NotOwned;
            }
            if (attempt.Status == AttemptStatus.InProgress)
            {
                return DomainErrors.Attempt.StillInProgress;
            }
            var answersDictionary = attempt.AttemptAnswers
                .ToDictionary(a => a.QuestionId);

            foreach (var gradeDto in request.Grades)
            {
                if (!answersDictionary.TryGetValue(gradeDto.QuestionId, out var attemptAnswer))
                {
                    return DomainErrors.Common.NotFound("Question",$"{gradeDto.QuestionId}");
                }

                attemptAnswer.Mark = gradeDto.Score;
                attemptAnswer.Feedback = gradeDto.Feedback;
            }
            attempt.Status = AttemptStatus.Reviewed;
            await _unitOfWork.CommitAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while grading submission with id {AttemptId}", request.Id);
            throw;
        }
    }
}
