using AutoMapper;
using LMS.Application.Features.Quizzes.Commands.CreateQuiz;
using LMS.Application.Features.Quizzes.Commands.QenerateQuestionsUsingAI;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Quizzes.Shared.Mappings;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        CreateMap<OptionRequest, Option>();

        CreateMap<QuestionRequest, Question>();

        CreateMap<CreateQuizCommand, Quiz>();
        CreateMap<Quiz, GetQuizDto>();
        CreateMap<AIQuestionGenerationJob, GetJobDto>();

        CreateMap<Quiz, GetAllQuizDto>();
        CreateMap<Question, QuestionDto>();

        CreateMap<Option, OptionDto>();

        CreateMap<QuestionGenerationFiles, QuestionGenerationFilesDto>();

        CreateMap<GenerateQuestionByAIRequest, GenerateQuestionsCommand>();

        CreateMap<CreateQuizCommand, Quiz>();
    }
}
