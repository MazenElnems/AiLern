using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using LMS.Application.Contracts.UnitOfWork;

namespace LMS.Application.Features.Attempts.Queries.GetAttemptInstructor;

public class GetStudentAnswersQueryHandler : IRequestHandler<GetStudentAnswersQuery, Result<AttemptResultDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;
    private readonly IMapper _mapper;

    public GetStudentAnswersQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<AttemptResultDto>> Handle(GetStudentAnswersQuery request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.Query
            .Include(a => a.Quiz)
                .ThenInclude(q => q.Course)
            .FirstOrDefaultAsync(a => a.Id == request.AttemptId, cancellationToken);

        if (attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        var course = attempt.Quiz.Course;

        if(course.InstructorId != instructorId)
            return DomainErrors.Common.Unauthorized("You are not authorized to view this attempt.");

        if (attempt.Status == AttemptStatus.InProgress)
            return DomainErrors.Attempt.StillInProgress;

        var studentAnswers = await _unitOfWork.Attempts.Query
            .ProjectTo<AttemptResultDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId, cancellationToken);

        return studentAnswers!;
    }
}
