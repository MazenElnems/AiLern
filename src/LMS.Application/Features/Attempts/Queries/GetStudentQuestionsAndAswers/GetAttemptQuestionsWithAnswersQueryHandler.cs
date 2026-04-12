using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Attempts.Queries.GetStudentQuestionsAndAswers;

public class GetAttemptQuestionsWithAnswersQueryHandler : IRequestHandler<GetAttemptQuestionsWithAnswersQuery, Result<List<AttemptQuestionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetAttemptQuestionsWithAnswersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<List<AttemptQuestionDto>>> Handle(GetAttemptQuestionsWithAnswersQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.Query
            .AsNoTracking()
            .Select(a => new {AttemptId = a.Id, a.StudentId, a.Quiz.ShuffleOptions, a.Quiz.ShuffleQuestions })
            .FirstOrDefaultAsync(a => a.AttemptId == request.AttemptId, cancellationToken);

        if(attempt == null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != userId)
            return DomainErrors.Attempt.NotOwned;

        var answersWithQuestions = await _unitOfWork.Answers.Query
            .AsNoTracking()
            .Where(a => a.AttemptId == request.AttemptId)
            .ProjectTo<AttemptQuestionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (attempt.ShuffleOptions)
            answersWithQuestions.ForEach(a => a.Options = a.Options?.OrderBy(_ => Guid.NewGuid()).ToList());
        
        if(attempt.ShuffleQuestions)
            answersWithQuestions = answersWithQuestions.OrderBy(_ => Guid.NewGuid()).ToList();

        return answersWithQuestions;
    }
}

