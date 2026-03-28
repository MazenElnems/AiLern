using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Attempts.Queries;

public class GetAttemptQuestionsQueryHandler : IRequestHandler<GetAttemptQuestionsQuery, Result<List<AttemptQuestionDto>>>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetAttemptQuestionsQueryHandler(IUnitOfWork unitOfWork, IPermissionService permissionService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _mapper = mapper;
    }

    public async Task<Result<List<AttemptQuestionDto>>> Handle(GetAttemptQuestionsQuery request, CancellationToken cancellationToken)
    {
        var attemptResult = await _permissionService.AuthorizeStudentAccessToAttemptAsync(request.AttemptId);
        if (!attemptResult.IsSuccess) return Result<List<AttemptQuestionDto>>.Failure(attemptResult.Error!);

        var attemptAnswers = await _unitOfWork.AttemptAnswers.GetAttemptAnswersByIdAsync(request.AttemptId);
        var dto = _mapper.Map<List<AttemptQuestionDto>>(attemptAnswers);

        return dto;
    }
}
