using AutoMapper;
using LMS.Application.Common.Interfaces;
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
    private readonly IPermissionService _permissionService;
    private readonly IUserContext _userContext;

    public GetQuizByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetQuizByIdQueryHandler> logger, IMapper mapper, IPermissionService permissionService, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<Result<GetQuizDto>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = _userContext.GetCurrentUser();

            var quizResult = await _permissionService.AuthorizeQuizAccessAsync(request.Id);
            if (!quizResult.IsSuccess) return Result<GetQuizDto>.Failure(quizResult.Error!);
            var quiz = quizResult.Value!;

            var quizWithQuestions = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.Id,
                includeProperties: [nameof(Quiz.Questions)]);

            var dto = _mapper.Map<GetQuizDto>(quizWithQuestions);

            if (user.IsInRole(UserRoles.Student))
            {
                if (quiz.Status != QuizStatus.Published)
                    return DomainErrors.Common.Forbidden("You do not have permissions to access this quiz.");

                dto.ShuffleQuestions = null;
                dto.ShuffleOptions = null;
                dto.Questions = null;
                dto.ShowResultOnClose = null;
                dto.CreatedAt = null;
                dto.Status = null;
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
