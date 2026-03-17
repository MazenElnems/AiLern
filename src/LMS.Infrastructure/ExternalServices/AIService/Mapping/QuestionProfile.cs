using AutoMapper;
using LMS.Domain.Entities.Quizzes;
using LMS.Infrastructure.ExternalServices.AIService.Responses;

namespace LMS.Infrastructure.ExternalServices.AIService.Mapping;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<AIQuestionGeneratedResponse, Question>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.QuestionType))
            .ForMember(dest => dest.Mark, opt => opt.MapFrom(src => 1.0))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options.Select((o, i) => new Option
            {
                OptionText = o,
                OptionNumber = i + 1,
                IsCorrect = o.StartsWith(src.CorrectAnswer, StringComparison.OrdinalIgnoreCase),
            })));
    }
}
