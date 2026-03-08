using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _backgroundJobService;

    public CreateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course == null)
            return DomainErrors.Course.NotFound(request.CourseId);

        if (user.Id != course.InstructorId)
            return DomainErrors.Course.NotOwned;

        if (course.CourseStatus != CourseStatus.Approved)
            return DomainErrors.Course.NotApproved;

        var quiz = _mapper.Map<Quiz>(request);

        int questionNumber = 1;
        quiz.Questions?.ForEach((question) =>
        {
            int optionNumber = 1;
            question.Options?.ForEach((option) =>
            {
                option.OptionNumber = optionNumber++;
            });
            question.Order = questionNumber++;
        });

        quiz.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Quizzes.InsertAsync(quiz);
        await _unitOfWork.CommitAsync();

        if (request.Status == QuizStatus.Scheduled)
            _backgroundJobService.Schedule<IQuizPublishSchedulerJob>(job => job.ExecuteAsync(quiz.Id), request.PublishedDate!.Value);

        return quiz.Id;
    }
}
