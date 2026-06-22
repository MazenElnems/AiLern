using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LMS.Application.Features.Quizzes.Queries.GetAiGradingDashboard;

public class GetAiGradingDashboardQueryHandler : IRequestHandler<GetAiGradingDashboardQuery, Result<AiGradingDashboardDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAiGradingDashboardQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AiGradingDashboardDto>> Handle(GetAiGradingDashboardQuery request, CancellationToken cancellationToken)
    {
        var answerWithAiGrading = await _unitOfWork.Answers.Query.AsNoTracking()
            .Where(a => a.AccuracyRating.HasValue)
            .Include(a=>a.Question)
            .ThenInclude(a=>a.Quiz)
            .ThenInclude(q=>q.Course)
            .ThenInclude(c=>c.Instructor).ToListAsync();

        var totalAiEvaluation = answerWithAiGrading.Count;

        var averageAiRating =totalAiEvaluation==0? 0 : answerWithAiGrading.Average(a => (int)a.AccuracyRating!);

        var satisfacationRate = (int)Math.Round((averageAiRating / 5.0) * 100);

        var lowQualityReviews = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.Poor || a.AccuracyRating == AccuracyRating.Fair);

        var poorCount = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.Poor);

        var fairCount = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.Fair);

        var goodCount = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.Good);

        var veryGoodCount = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.VeryGood);

        var excellentCount = totalAiEvaluation == 0 ? 0 : answerWithAiGrading
            .Count(a => a.AccuracyRating == AccuracyRating.Excellent);

        var instructorFeedbackOnAiGrading = answerWithAiGrading
            .GroupBy(a => a.FeedbackThemes)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Count()
            );

        var lowestRatedAiEvaluations = answerWithAiGrading.Where(a => a.AccuracyRating == AccuracyRating.Poor || a.AccuracyRating == AccuracyRating.Fair)
            .Select(a => new LowestRated
            {
                Rating = (int)a.AccuracyRating!,
                QuestionText = a.Question.QuestionText,
                CourseName = a.Question.Quiz.Course.Name,
                AiScore = a.Mark,
                AiFeedback = a.Feedback!,
                InstructorName = a.Question.Quiz.Course.Instructor.FullName
            }).ToList();


        var result = new AiGradingDashboardDto
        {
            AverageAiRating = averageAiRating,
            ExcellentCount = excellentCount,
            FairCount = fairCount,
            GoodCount = goodCount,
            PoorCount = poorCount,
            VeryGoodCount = veryGoodCount,
            InstructorFeedbackOnAiGrading = instructorFeedbackOnAiGrading!,
            LowestRatedAiEvaluations = lowestRatedAiEvaluations,
            LowQualityReviews = lowQualityReviews,
            SatisfacationRate = satisfacationRate,
            TotalAiEvaluation = totalAiEvaluation

        };

        return result;
    }
}
