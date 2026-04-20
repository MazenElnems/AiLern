using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuizStatus;

public class UpdateQuizStatusCommandHandler : IRequestHandler<UpdateQuizStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public UpdateQuizStatusCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpdateQuizStatusCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId, 
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        if(request.Status == quiz.Status)
            return Result.Success("Quiz status is already set to the requested value.");

        if(quiz.AvailableFrom < DateTime.UtcNow)
            return DomainErrors.Quiz.QuizStarted;

        if(request.Status == QuizStatus.Published && !await _unitOfWork.Questions.AnyAsync(q => q.QuizId == request.QuizId))
            return DomainErrors.Quiz.CannotPublishEmptyQuiz;

        quiz.Status = request.Status;
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Quiz status updated successfully.");
    }
}
