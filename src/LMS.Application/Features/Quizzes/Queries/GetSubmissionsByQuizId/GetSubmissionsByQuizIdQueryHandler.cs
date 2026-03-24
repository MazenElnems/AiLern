using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;

public class GetSubmissionsByQuizIdQueryHandler : IRequestHandler<GetSubmissionsByQuizIdQuery, Result<List<GetSubmissionsByQuizIdDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSubmissionsByQuizIdQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GetSubmissionsByQuizIdDto>>> Handle(GetSubmissionsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();
        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId, includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != user.Id)
            return DomainErrors.Quiz.NotOwned;

        //var submissions = await _unitOfWork.Attempts
        //    .FilterAsync(s => s.QuizId == request.QuizId && s.Status == AttemptStatus.Submitted, includeProperties: [nameof(Attempt.Student), nameof(Attempt.AttemptAnswers)]);

        var submissions = await _unitOfWork.Attempts.Query
            .Where(a => a.QuizId == request.QuizId && a.Status != AttemptStatus.InProgress)
            .Select(a => new GetSubmissionsByQuizIdDto
            {
                Id = a.Id,
                AttemptNumber = a.AttemptNumber,
                Score = a.AttemptAnswers.Sum(a=>a.Mark),
                StartAt = a.StartAt,
                Status = a.Status,
                StudentId = a.StudentId,
                StudentName = a.Student.FullName,
                SubmittedAt = a.SubmittedAt,
                TimeSpent = a.TimeSpent
            }).ToListAsync();

        //var dto = _mapper.Map<List<GetSubmissionsByQuizIdDto>>(submissions);
        return submissions;
    }
}
