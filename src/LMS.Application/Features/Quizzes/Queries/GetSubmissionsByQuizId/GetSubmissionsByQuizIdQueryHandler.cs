using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Models.Responses;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetSubmissionsByQuizId;

public class GetSubmissionsByQuizIdQueryHandler(
    IUserContext userContext,
    IUnitOfWork unitOfWork,
    IMapper mapper
    ) : IRequestHandler<GetSubmissionsByQuizIdQuery, Result<PaginationResult<GetSubmissionsByQuizIdDto>>>
{
    private readonly IUserContext _userContext = userContext;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<PaginationResult<GetSubmissionsByQuizIdDto>>> Handle(GetSubmissionsByQuizIdQuery request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        var query = _unitOfWork.Attempts.Query
            .Where(a => a.QuizId == request.QuizId && (a.Status == request.Status || request.Status==null));

        var totalResult = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.SubmittedAt)
            .Skip(request.PageSize * (request.PageNo - 1))
            .Take(request.PageSize)
            .ProjectTo<GetSubmissionsByQuizIdDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new PaginationResult<GetSubmissionsByQuizIdDto>(
            request.PageNo,
            request.PageSize,
            totalResult,
            items
        );
    }
}
