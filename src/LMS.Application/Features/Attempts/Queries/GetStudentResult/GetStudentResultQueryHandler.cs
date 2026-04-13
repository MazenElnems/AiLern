using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Attempts.Queries.GetAttempt;

public class GetStudentResultQueryHandler : IRequestHandler<GetStudentResultQuery, Result<AttemptResultDto>>
{
    private readonly IUserContext _user;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentResultQueryHandler(IUserContext user, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _user = user;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AttemptResultDto>> Handle(GetStudentResultQuery request, CancellationToken cancellationToken)
    {
        var userId = _user.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.GetAsync(a => a.Id == request.AttemptId,
            includeProperties: [nameof(Attempt.Quiz)]);

        if (attempt == null) 
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != userId)
            return DomainErrors.Attempt.NotOwned;

        if (attempt.Quiz.AvailableUntil > DateTime.UtcNow)
            return DomainErrors.Attempt.QuizNotFinshYet;

        if (!attempt.Quiz.ShowResultOnClose && attempt.Status != AttemptStatus.Reviewed)
            return DomainErrors.Attempt.AttemptNotReviewedYet;

        var studentResult = await _unitOfWork.Attempts.Query
            .AsNoTracking()
            .ProjectTo<AttemptResultDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId, cancellationToken);

        return studentResult!;
    }
}

