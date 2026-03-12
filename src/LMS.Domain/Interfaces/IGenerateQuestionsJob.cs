using LMS.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Interfaces
{
    public interface IGenerateQuestionsJob
    {
        Task ExecuteAsync(Guid jobId,
            Guid quizId,
            List<string> materialFileIds,
            List<IFormFile> files,
            int questionsCount,
            Dictionary<QuestionType, int> questionTypeCounts,
            Dictionary<QuestionDifficultyLevels, float> questionDifficultyPercents,
            CancellationToken token,
            string? query = null);
    }
}
