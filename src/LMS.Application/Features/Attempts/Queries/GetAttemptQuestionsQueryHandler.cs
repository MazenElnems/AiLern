using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries;

public class GetAttemptQuestionsQueryHandler : IRequestHandler<GetAttemptQuestionsQuery, Result<List<AttemptQuestionDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAttemptQuestionsQueryHandler(IUnitOfWork unitOfWork, IUserContext userContext, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
        _mapper = mapper;
    }

    public async Task<Result<List<AttemptQuestionDto>>> Handle(GetAttemptQuestionsQuery request, CancellationToken cancellationToken)
    {
        var user = _userContext.GetCurrentUser();

        var attempt = await _unitOfWork.Attempts.GetByIdAsync(request.AttemptId);

        if(attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != user.Id)
            return DomainErrors.Common.Forbidden("You are not allowed to access this attempt.");

        var attemptAnswers = await _unitOfWork.AttemptAnswers.GetAttemptAnswersByIdAsync(request.AttemptId);
        var dto = _mapper.Map<List<AttemptQuestionDto>>(attemptAnswers);

        return dto;
    }
}
