using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.GradeSubmission;

public class GradeSubmissionCommandHandler : IRequestHandler<GradeSubmissionCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public GradeSubmissionCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.GetAsync(a => a.Id == request.Id,
            includeProperties: [nameof(Attempt.Answers)]);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.Id);

        var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == attempt.QuizId, includeProperties: [nameof(Quiz.Course)]);
        if (quiz == null)
            return DomainErrors.Quiz.NotFound(attempt.QuizId);
        if (quiz.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;

        if (attempt.Status == AttemptStatus.InProgress)
            return DomainErrors.Attempt.StillInProgress;

        var answersDictionary = attempt.Answers
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
