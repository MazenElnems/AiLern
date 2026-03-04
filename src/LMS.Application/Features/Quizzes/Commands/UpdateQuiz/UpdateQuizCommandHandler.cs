using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Result<GetAllQuizDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<GetAllQuizDto>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;
        var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == request.Id,
    includeProperties: [nameof(Quiz.Course)]);
        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.Id);
        if (quiz.Course.InstructorId != userId)
            return DomainErrors.Course.NotOwned;
        quiz.Title = request.Title;
        quiz.Description = request.Description;
        if (quiz.IsPublished == false)
        {
            if (request.AvailableUntil <= request.AvailableFrom)
                return DomainErrors.Quiz.InvalidAvailabilityRange;
            quiz.AvailableFrom = request.AvailableFrom;
            quiz.AvailableUntil = request.AvailableUntil;
            quiz.ShowCorrectAnswersAfterClose = request.ShowCorrectAnswersAfterClose;
            quiz.IsPublished = request.IsPublished;
            quiz.ShuffleQuestions = request.ShuffleQuestions;
            quiz.ShuffleOptions = request.ShuffleOptions;
            quiz.MaximumAttempts = request.MaximumAttempts;
            quiz.TotalPoints = request.TotalPoints;
        }
        else
        {
            return DomainErrors.Quiz.AlreadyPublished;
        }

        var dto = _mapper.Map<GetAllQuizDto>(quiz);
        await _unitOfWork.CommitAsync();
        return Result<GetAllQuizDto>.Success(dto, "quiz updated successfully.");

    }
}
