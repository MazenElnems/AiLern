using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Constants;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
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
        private readonly IUserContext _userContext;

        public GetAllQuizzesByCourseIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAllQuizzesByCourseIdQueryHandler> logger, IMapper mapper, IUserContext userContext)
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
                var user =  _userContext.GetCurrentUser();

                var course = await _unitOfWork.Courses.GetAsync(c => c.Id ==  request.CourseId, 
                    includeProperties: [nameof(Course.Quizzes)]);

                if (course == null)
                    return DomainErrors.Course.NotFound(request.CourseId);

                if (user.IsInRole(UserRoles.Instructor) && user.Id != course.InstructorId)
                    return DomainErrors.Course.NotOwned;

                else if (user.IsInRole(UserRoles.Student) && !await _unitOfWork.Enrollments.IsEnrolledAsync(request.CourseId, user.Id))
                    return DomainErrors.Common.Forbidden("Can't access this course");

                Expression<Func<Quiz, bool>> perdicate = user.IsInRole(UserRoles.Student) 
                    ?q => q.CourseId == request.CourseId  && q.Status == QuizStatus.Published 
                    :q => q.CourseId == request.CourseId;

                var quizzes = await _unitOfWork.Quizzes.FilterAsync(perdicate);

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
