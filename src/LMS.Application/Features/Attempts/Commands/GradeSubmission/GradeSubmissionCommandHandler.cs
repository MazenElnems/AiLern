using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public GradeSubmissionCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.Attempts.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Attempt.AttemptAnswers)]);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.Id);

        var quizResult = await _permissionService.AuthorizeInstructorAccessToQuizAsync(attempt.QuizId);
        if (!quizResult.IsSuccess) return quizResult.Error!;

        if (attempt.Status == AttemptStatus.InProgress)
            return DomainErrors.Attempt.StillInProgress;

        var answersDictionary = attempt.AttemptAnswers
            .ToDictionary(a => a.QuestionId);

        foreach (var gradeDto in request.Grades)
        {
            if (!answersDictionary.TryGetValue(gradeDto.QuestionId, out var attemptAnswer))
                return DomainErrors.Common.NotFound("Question", $"{gradeDto.QuestionId}");

            attemptAnswer.Mark = gradeDto.Score;
            attemptAnswer.Feedback = gradeDto.Feedback;
        }

        attempt.Status = request.Status;
        await _unitOfWork.CommitAsync();
        return Result.Success("Attempt graded successfully.");
    }
}
