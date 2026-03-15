using LMS.Application.Common.Results.Generic;
using LMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;

public class GenerateQuestionsCommand : IRequest<Result<Guid>>
{
    [JsonIgnore]
    public Guid QuizId { get; set; }

    // var qFiles = Db;

    // qFiles.RemoveRange(FileIds.Except(qFiles.Select(f => f.Id)));
    // qFiles.AddRange( FileIds.Except(qFiles.Select(f => f.Id)).Select(id => new QuestionGenerationFiles { Id = id, QuizId = QuizId }));

    public List<Guid> FileIds { get; set; } // MaterialIDs + QuestionGenerationFiles
    public List<IFormFile> NewUploadedFiles { get; set; }
    public int QuestionsCount { get; set; }
    public Dictionary<QuestionType, int> QuestionTypeCounts { get; set; }
    public Dictionary<QuestionDifficultyLevels, float> QuestionDifficultyPercents { get; set; }
    public string? Query { get; set; }
}
