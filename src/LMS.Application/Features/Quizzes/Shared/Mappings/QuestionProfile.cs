using AutoMapper;
using LMS.Application.Common.Models.Responses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Shared.Mappings;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<AIQuestionGeneratedResponse, Question>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.QuestionType))
            .ForMember(dest => dest.Mark, opt => opt.MapFrom(src => 1.0))
            .ForMember(dest => dest.IsAIGenerated, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(src => false))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => 0))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options.Select((o, i) => new Option
            {
                OptionText = (src.QuestionType == QuestionType.MCQ) ? new string(o.Skip(2).ToArray()) : o,
                OptionNumber = i + 1,
                IsCorrect = o.StartsWith(src.CorrectAnswer ?? string.Empty, StringComparison.OrdinalIgnoreCase),
            })));
    }
}
