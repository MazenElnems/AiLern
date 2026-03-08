using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _backgroundJobService;

    public UpdateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result<Guid>> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
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
        quiz.AvailableUntil = request.AvailableUntil;
        quiz.ShowResultOnClose = request.ShowResultOnClose;
        quiz.ShuffleQuestions = request.ShuffleQuestions;
        quiz.ShuffleOptions = request.ShuffleOptions;
        quiz.MaximumAttempts = request.MaximumAttempts;

        var quizStarted = DateTime.UtcNow > quiz.AvailableFrom;

        if (!quizStarted)
        {
            quiz.AvailableFrom = request.AvailableFrom;
            quiz.Status = request.Status;

            if (request.Status == QuizStatus.Published)
                quiz.PublishedAt = DateTime.UtcNow;
            else if (request.Status == QuizStatus.Draft)
                quiz.PublishedAt = null;
            else if (request.Status == QuizStatus.Scheduled)
            {
                quiz.PublishedAt = null;
                _backgroundJobService.Schedule<IQuizPublishSchedulerJob>((job) => job.ExecuteAsync(quiz.Id), request.PublishedDate!.Value);
            }

            if(request.Questions != null)
            {
                var questionOrder = 1;
                request.Questions.ForEach(questionRequest =>
                {
                    var question = _mapper.Map<Question>(questionRequest);
                    question.QuizId = quiz.Id;  

                    int optionNumber = 1;
                    question.Options.ForEach(o =>
                    {
                        o.OptionNumber = optionNumber++;
                        o.QuestionId = question.Id;
                    });

                    question.Order = questionOrder++;
                    _unitOfWork.Questions.Update(question);
                });
            }
        }

        await _unitOfWork.CommitAsync();
        return Result<Guid>.Success(quiz.Id, "quiz updated successfully.");
    }
}
