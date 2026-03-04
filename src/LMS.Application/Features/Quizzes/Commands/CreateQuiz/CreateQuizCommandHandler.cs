using AutoMapper;
using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz
{
    public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public CreateQuizCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<Result<QuizDto>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
        {
            var user = _userContext.GetCurrentUser();
            var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);

            if (course == null)
                return DomainErrors.Course.NotFound(request.CourseId);

            if (user.Id != course.InstructorId)
                return DomainErrors.Common.Forbidden("You do not have permission to create a quiz for this course.");

            if (course.CourseStatus != CourseStatus.Approved)
                return DomainErrors.Course.NotApproved;



            var quiz = _mapper.Map<Quiz>(request);

            quiz.CreatedAt = DateTime.UtcNow;
            quiz.IsPublished = false;
            quiz.UpdatedAt = null;

            await _unitOfWork.Quizzes.InsertAsync(quiz);
            await _unitOfWork.CommitAsync();

            var quizDto = _mapper.Map<QuizDto>(quiz);

            return Result<QuizDto>.Success(quizDto, "The quiz was created successfully.");
        }
    }
}
