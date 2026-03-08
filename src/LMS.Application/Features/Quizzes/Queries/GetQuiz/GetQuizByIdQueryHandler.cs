using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;


namespace LMS.Application.Features.Quizzes.Queries.GetQuiz;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, Result<GetQuizDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetQuizByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetQuizByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetQuizByIdQueryHandler> logger, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<GetQuizDto>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var quiz = await _unitOfWork.Quizzes.GetAsync(a => a.Id == request.Id,
                includeProperties: [nameof(Quiz.Questions) , nameof(Quiz.Course)] );

            if (quiz == null)
                return DomainErrors.Quiz.NotFound(request.Id);

                var dto = _mapper.Map<GetQuizDto>(quiz);
            if (user.IsInRole(UserRoles.Instructor))
            {
                if (quiz.Course.InstructorId != user.Id)
                    return DomainErrors.Course.NotOwned;
            }
            else if (user.IsInRole(UserRoles.Student))
            {
                var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(quiz.Course.Id, user.Id);
                if (!isEnrolled)
                    return DomainErrors.Course.NotEnrolled;
                if (quiz.Status != QuizStatus.Published)
                    return DomainErrors.Quiz.NotFound(request.Id);
                dto.ShuffleQuestions = null;
                dto.ShuffleOptions = null;
                dto.Questions = null;
                dto.ShowResultOnClose = null;
                dto.CreatedAt = null;
                dto.Status = null;
                dto.IsPublished = null;

            }
            return Result<GetQuizDto>.Success(dto);


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving quiz.");
            throw;
        }
    }
}
