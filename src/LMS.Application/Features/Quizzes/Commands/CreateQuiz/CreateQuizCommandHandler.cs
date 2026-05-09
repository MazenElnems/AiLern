using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Notification;
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
    private readonly INotificationService _notificationService;

    public CreateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

        if (course is null)
            return Result<Guid>.Failure(DomainErrors.Course.NotFound(request.CourseId));

        if (course.InstructorId != instructorId)
            return Result<Guid>.Failure(DomainErrors.Course.NotOwned);

        var quiz = _mapper.Map<Quiz>(request);

        quiz.Status = QuizStatus.Draft;
        quiz.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Quizzes.InsertAsync(quiz);
        await _unitOfWork.CommitAsync(cancellationToken);


        if(quiz.Status == QuizStatus.Published)
        {
            await _notificationService.NotifyAsync(
                quiz.CourseId,
                $"{course.Name}: New Quiz",
                $"\"{quiz.Title}\" is now available. Start solving now!",
                NotificationType.NewQuizAdded,
                $"https://www.ailern.me/quizzes/{quiz.Id}",
                "Start Quiz"
            );
        }

        return Result<Guid>.Success(quiz.Id, "quiz created successfully.");
    }
}
