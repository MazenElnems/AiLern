using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetAiQuestionGenerationDashboard;

public class GetAiQuestionGenerationDashboardQueryHandler : IRequestHandler<GetAiQuestionGenerationDashboardQuery, Result<AiQuestionGenerationDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAiQuestionGenerationDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AiQuestionGenerationDashboardDto>> Handle(GetAiQuestionGenerationDashboardQuery request, CancellationToken cancellationToken)
    {
        var questions = await _unitOfWork.Questions.Query.Where(q => q.IsRelated.HasValue).ToListAsync();
        var totalValidation = questions.Count;
        var relatedQuestions = questions.Where(q => q.IsRelated == true).Count();
        var unRelatedQuestions = questions.Where(q => q.IsRelated != true).Count();

        var topicAlignmentRate = (int)Math.Round((relatedQuestions /(totalValidation*1.0)) * 100);

        var overviewByCourses = await _unitOfWork.Questions.Query.AsNoTracking()
            .Include(q => q.Quiz)
            .ThenInclude(q => q.Course)
            .Select(q => new { CourseId = q.Quiz.Course.Id, CourseName = q.Quiz.Course.Name, GeneratedByAi = q.IsAIGenerated, IsRelated = q.IsRelated })
            .GroupBy(q => q.CourseName)
            .Select(qg => new QuestionValidatioOverviewByCourse { CourseName = qg.Key, 
                GeneratedByAi = qg.Count(x => x.GeneratedByAi), 
                RelatedCount = qg.Count(x => x.IsRelated == true), 
                UnRelatedCount = qg.Count(x => x.IsRelated == false) })
            .ToListAsync();

        var result = new AiQuestionGenerationDashboardDto
        {
            TotalValidation = totalValidation,
            TopicAlignmentRate = topicAlignmentRate,
            RelatedQuestions = relatedQuestions,
            UnrelatedQuestions = unRelatedQuestions,
            OverviewByCourses = overviewByCourses
        };

        return result;




    }
}
