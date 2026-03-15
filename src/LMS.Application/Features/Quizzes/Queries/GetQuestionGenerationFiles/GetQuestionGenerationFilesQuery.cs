using LMS.Application.Common.Results;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetQuestionGenerationFiles;

public class GetQuestionGenerationFilesQuery(Guid quizId) : IRequest<Result<List<QuestionGenerationFilesDto>>>
{
    public Guid QuizId { get; } = quizId;
}
