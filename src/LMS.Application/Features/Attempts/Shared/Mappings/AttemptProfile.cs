using AutoMapper;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Attempts.Shared.Mappings;

public class AttemptProfile : Profile
{
    public AttemptProfile()
    {
        CreateMap<Attempt, GetAttemptByIdDto>();
        CreateMap<AttemptAnswer, AttemptResultDto>();
        CreateMap<Option, AttemptOptionDto>()
            .ForMember(dto => dto.Option, opt => opt.MapFrom(src => src.OptionText));

        CreateMap<AttemptAnswer, AttemptQuestionDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Question.Id))
            .ForMember(dto => dto.Question, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dto => dto.Type, opt => opt.MapFrom(src => src.Question.Type))
            .ForMember(dto => dto.Instructions, opt => opt.MapFrom(src => src.Question.Instructions))
            .ForMember(dto => dto.Options, opt => opt.MapFrom(src => src.Question.Options))
            .ForMember(dto => dto.WrittenAnswer, opt => opt.MapFrom(src => src.WrittenAnswer))
            .ForMember(dto => dto.BooleanAnswer, opt => opt.MapFrom(src => src.BooleanAnswer))
            .ForMember(dto => dto.OptionNumber, opt => opt.MapFrom(src => src.OptionNumber));
    }
}
