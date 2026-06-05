using AutoMapper;
using LMS.Application.Common.Models.Request;
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

        CreateMap<AIGradingCriteria, QuestionCriteriaDto>();

        CreateMap<Quiz, GetAllQuizDto>();

        CreateMap<Question, QuestionDto>()
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ModelAnswer, opt => opt.MapFrom(src => src.AIGradingReferenceAnswer));

        CreateMap<Option, OptionDto>();

        CreateMap<GenerateQuestionByAIRequest, GenerateQuestionsCommand>();

        CreateMap<CreateQuizCommand, Quiz>();

        CreateMap<GenerateQuestionsCommand, AIQuizGenerationRequest>()
            .ForMember(dest => dest.ProjectIds, opt => opt.MapFrom(src => src.FileIds.Select(f => f.ToString())))
            .ForMember(dest => dest.QuestionsNumber, opt => opt.MapFrom(src => src.QuestionsCount))
            .ForMember(dest => dest.QuestionsTypes, opt => opt.MapFrom(src => src.QuestionTypeCounts))
            .ForMember(dest => dest.DifficultyLevels, opt => opt.MapFrom(src => src.QuestionDifficultyPercents));
    }
}
