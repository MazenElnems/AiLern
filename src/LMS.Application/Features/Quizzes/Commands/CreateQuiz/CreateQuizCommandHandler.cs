using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
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
        var userId = _userContext.GetCurrentUser().Id;
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        if (course is null)
            return Result<Guid>.Failure(DomainErrors.Course.NotFound(request.CourseId));
        if (course.InstructorId != userId)
            return Result<Guid>.Failure(DomainErrors.Course.NotOwned);

        var quiz = _mapper.Map<Quiz>(request.Quiz);

        if(quiz.Status == QuizStatus.Published)
            quiz.PublishedAt = DateTime.UtcNow;

        quiz.CreatedAt = DateTime.UtcNow;

        if (quiz.Status == QuizStatus.Scheduled)
            quiz.PublishBackgroundJobId = _backgroundJobService.Schedule<IQuizPublishSchedulerJob>(job => job.ExecuteAsync(quiz.Id), request.Quiz.PublishedDate!.Value);

        await _unitOfWork.Quizzes.InsertAsync(quiz);
        await _unitOfWork.CommitAsync();

        return Result<Guid>.Success(quiz.Id, "quiz created successfully.");
    }
}
