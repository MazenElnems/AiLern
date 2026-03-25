using LMS.Application.Common.Results;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Commands.SubmitAttempt;

public class SubmitAttemptCommandHandler : IRequestHandler<SubmitAttemptCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBackgroundJobService _backgroundJobService;

    public SubmitAttemptCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var attempt = await _unitOfWork.Attempts.GetByIdAsync(request.AttemptId);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        //var attemptAnswers = await _unitOfWork.AttemptAnswers.FilterAsync(a => a.AttemptId == request.AttemptId && a.Question.Type == QuestionType.Written,
        //    includeProperties: [nameof(AttemptAnswer.Question)]);

        //foreach (var answer in attemptAnswers)
        //{
        //    var question = answer.Question;
        //    if (question.Type == QuestionType.MCQ)
        //    {
        //        if(answer.OptionNumber == question.Options.First(q => q.IsCorrect).OptionNumber)
        //        {
        //            answer.Mark = question.Mark;
        //        }
        //    }

        //    else if(question.Type == QuestionType.TrueFalse)
        //    {
        //        if (answer.BooleanAnswer?.ToLower() == question.Options.First(q => q.IsCorrect).OptionText.ToLower())
        //        {
        //            answer.Mark = question.Mark;
        //        }
        //    }
        //}

        attempt.Submit();
        await _unitOfWork.CommitAsync();

        _backgroundJobService.Delete(attempt.AutoSubmitJobId);

        return Result.Success("Submit Successfully");
    }
}
