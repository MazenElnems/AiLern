using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;

namespace LMS.Application.Features.Quizzes.Commands.UpdateQuestionGradingCriteria;

public class UpdateQuestionGradingConfigCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    : IRequestHandler<UpdateQuestionGradingConfigCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserContext _userContext = userContext;

    public async Task<Result> Handle(UpdateQuestionGradingConfigCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId,
            includeProperties: [nameof(Quiz.Course)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != currentUserId)
            return DomainErrors.Quiz.NotOwned;

        var question = await _unitOfWork.Questions
            .GetAsync(q => q.Id == request.QuestionId && q.QuizId == request.QuizId,
            includeProperties: [nameof(Question.Criterias)]);

        if (question == null)
            return DomainErrors.Common.NotFound("Question", request.QuestionId.ToString());

        var gradingConfigDto = request.GradingConfigDto;

        if(question.Mark != gradingConfigDto.Criteria.Sum(c => c.Mark))
            return DomainErrors.Common.Validation("Mark", "The sum of criteria marks must equal the question mark.");

        question.AIGradingReferenceAnswer = gradingConfigDto.ModelAnswer;
        var existingCriteria = question.Criterias.ToDictionary(c => c.Id);

        foreach (var criterionDto in gradingConfigDto.Criteria)
        {
            if (criterionDto.Id == null)
            {
                // New criterion, add it
                var newCriterion = new AIGradingCriteria
                {
                    Id = Guid.NewGuid(),
                    QuestionId = question.Id,
                    Criterion = criterionDto.Criterion,
                    Mark = criterionDto.Mark
                };
                question.Criterias.Add(newCriterion);
            }
            else if (existingCriteria.TryGetValue(criterionDto.Id.Value, out var existingCriterion))
            {
                // Existing criterion, update it
                existingCriterion.Criterion = criterionDto.Criterion;
                existingCriterion.Mark = criterionDto.Mark;
            }
        }

        // Remove criteria that are in the database but not in the request
        var criteriaToRemoveIds = existingCriteria.Keys
            .Except(gradingConfigDto.Criteria.Select(c => c.Id ?? Guid.Empty));

        question.Criterias.RemoveAll(c => criteriaToRemoveIds.Contains(c.Id));

        await _unitOfWork.CommitAsync(cancellationToken);
        return Result.Success("Question grading configuration updated successfully.");
    }
}
