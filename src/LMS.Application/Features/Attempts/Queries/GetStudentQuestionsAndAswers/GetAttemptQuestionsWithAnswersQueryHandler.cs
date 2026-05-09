using AutoMapper;
using AutoMapper.QueryableExtensions;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Enums;
using LMS.Domain.Errors;
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
        var studentId = _userContext.GetCurrentUser().Id;

        var attempt = await _unitOfWork.Attempts.Query
            .Where(a => a.Id == request.AttemptId)
            .Select(a => new
            {
                a.Id,
                a.StudentId,
                a.Status,
                a.ShuffledQuestionIds,
                a.Quiz.ShuffleQuestions,
                a.Quiz.ShuffleOptions
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (attempt is null)
            return DomainErrors.Attempt.NotFound(request.AttemptId);

        if (attempt.StudentId != studentId)
            return DomainErrors.Attempt.NotOwned;

        if (attempt.Status != AttemptStatus.InProgress)
            return DomainErrors.Attempt.NotInProgress;

        var questions = await _unitOfWork.Answers.Query
            .Where(a => a.AttemptId == request.AttemptId)
            .ProjectTo<AttemptQuestionDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (attempt.ShuffleQuestions && attempt.ShuffledQuestionIds != null)
        {
            var questionMap = attempt.ShuffledQuestionIds
                .Select((id, index) => new { id, index })
                .ToDictionary(x => x.id, x => x.index);

            questions = questions
                .OrderBy(q => questionMap.TryGetValue(q.Id, out var order) ? order : int.MaxValue)
                .ToList();
        }
        else
        {
            // Default Ordering
            questions = questions
                .OrderBy(q => q.Order).ToList();
        }

        foreach (var question in questions)
        {
            if (question.Options == null) continue;

            if (attempt.ShuffleOptions && question.ShuffledOptionIds != null)
            {
                var optionMap = question.ShuffledOptionIds
                    .Select((id, index) => new { id, index })
                    .ToDictionary(x => x.id, x => x.index);

                question.Options = question.Options
                    .OrderBy(o => optionMap.TryGetValue(o.OptionId, out var order) ? order : int.MaxValue)
                    .ToList();

                question.ShuffledOptionIds = null;
            }
            else
            {
                // Default Ordering
                question.Options = question.Options
                    .OrderBy(o => o.Order)
                    .ToList();
            }
        }

        return questions;
    }
}
