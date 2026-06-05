using AutoMapper;
using LMS.Application.Common.Models.Request;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Infrastructure.ExternalServices.AIService.Mapping;

internal class AttemptAnswersProfile : Profile
{
    public AttemptAnswersProfile()
    {
        CreateMap<Answer, StudentAttemptAnswers>()
            .ForMember(dest => dest.StudentAnswer, opt => opt.MapFrom(src => src.OptionId == null ? src.WrittenAnswer : src.Option.OptionText));

        CreateMap<Attempt, StudentBatchAnswer>()
            .ForMember(dest => dest.AttemptId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers));
    }
}
