using LMS.Application.Contracts.Repositories;
using LMS.Application.Features.Dashboards.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Persistence.Repositories;

internal class QuizRepository : BaseRepository<Quiz>, IQuizRepository
{
    private readonly AppDbContext _context;

    public QuizRepository(AppDbContext context)
        :base(context)
    {
        _context = context;
    }

    public async Task<List<QuizStatisticsDto>> GetQuizStatisticsForCourseAsync(int courseId)
    {
        return await _context.Attempts
            .Where(a => a.Status == AttemptStatus.Reviewed &&
                        a.Quiz.AvailableUntil < DateTime.UtcNow &&
                        a.Quiz.CourseId == courseId
            )
            .GroupBy(a => new { a.QuizId, a.Quiz.Title })
            .Select(a => new QuizStatisticsDto
            {
                QuizId = a.Key.QuizId,        //    return await _context.Attempts
                QuizTitle = a.Key.Title,        //.Where(a => a.Status == AttemptStatus.Reviewed &&
                AverageScore = a.Average(a => a.Answers.Sum(x => x.Mark)),        //            a.Quiz.AvailableUntil < DateTime.UtcNow &&
                AverageTimeSpentInMinutes = a.Average(a => EF.Functions.DateDiffMinute(a.StartAt, a.SubmittedAt)) // EF can't translate this to TSQL        //            a.Quiz.CourseId == courseId
            }).ToListAsync();        
    }
}
