using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobService _backgroundJobService;

    public CreateQuizCommandHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper, IBackgroundJobService backgroundJobService)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
        _backgroundJobService = backgroundJobService;
    }

    public async Task<Result<Guid>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(request.CourseId);
        if (!courseResult.IsSuccess) return Result<Guid>.Failure(courseResult.Error!);

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
