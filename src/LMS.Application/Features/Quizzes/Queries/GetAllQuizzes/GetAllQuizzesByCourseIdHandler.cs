using AutoMapper;
using LMS.Application.Common.Models.Responses;
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
using System.Linq.Expressions;

namespace LMS.Application.Features.Quizzes.Queries.GetAllQuizzes
{
    public class GetAllQuizzesByCourseIdHandler : IRequestHandler<GetAllQuizzesByCourseIdQuery, Result<List<GetAllQuizDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetAllQuizzesByCourseIdHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public GetAllQuizzesByCourseIdHandler(IUnitOfWork unitOfWork, ILogger<GetAllQuizzesByCourseIdHandler> logger, IMapper mapper, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<Result<List<GetAllQuizDto>>> Handle(GetAllQuizzesByCourseIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Expression<Func<Quiz, bool>> predicate = c => true;
                var user =  _userContext.GetCurrentUser();

                if (user.IsInRole(UserRoles.Instructor))
                {
                    predicate = c => c.Course.InstructorId == user.Id;
                }
                else if (user.IsInRole(UserRoles.Student))
                {
                    var isEnrolled = await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id);
                    predicate = c => c.Status == QuizStatus.Published && isEnrolled;
                }

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
