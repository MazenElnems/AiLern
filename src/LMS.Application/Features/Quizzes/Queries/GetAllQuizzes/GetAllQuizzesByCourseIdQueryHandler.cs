using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes
{
    public class GetAllQuizzesByCourseIdQueryHandler : IRequestHandler<GetAllQuizzesByCourseIdQuery, Result<List<GetAllQuizDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllQuizzesByCourseIdQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;
        private readonly IUserContext _userContext;

        public GetAllQuizzesByCourseIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllQuizzesByCourseIdQueryHandler> logger, IMapper mapper, IPermissionService permissionService, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _permissionService = permissionService;
            _userContext = userContext;
        }

        public async Task<Result<List<GetAllQuizDto>>> Handle(GetAllQuizzesByCourseIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = _userContext.GetCurrentUser();

                var courseResult = await _permissionService.AuthorizeCourseAccessAsync(request.CourseId);
                if (!courseResult.IsSuccess) return Result<List<GetAllQuizDto>>.Failure(courseResult.Error!);

                Expression<Func<Quiz, bool>> predicate = user.IsInRole(UserRoles.Student)
                    ? q => q.CourseId == request.CourseId && q.Status == QuizStatus.Published
                    : q => q.CourseId == request.CourseId;

                var quizzes = await _unitOfWork.Quizzes.FilterAsync(predicate);

                var dto = _mapper.Map<List<GetAllQuizDto>>(quizzes);

                return Result<List<GetAllQuizDto>>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving quizzes.");
                throw;
            }
        }
    }
}
