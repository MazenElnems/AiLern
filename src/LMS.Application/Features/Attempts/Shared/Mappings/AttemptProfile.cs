using AutoMapper;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Application.Features.Quizzes.Shared.Requests;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Attempts.Shared.Mappings;

public class AttemptProfile : Profile
{
    public AttemptProfile()
    {
        CreateMap<Attempt, AttemptResultDto>();

        CreateMap<Answer, AnswerDto>();

        CreateMap<Option, AttemptOptionDto>()
            .ForMember(dto => dto.Option, opt => opt.MapFrom(src => src.OptionText));

        CreateMap<Answer, AttemptQuestionDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Question.Id))
            .ForMember(dto => dto.Question, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dto => dto.Type, opt => opt.MapFrom(src => src.Question.Type))
            .ForMember(dto => dto.Instructions, opt => opt.MapFrom(src => src.Question.Instructions))
            .ForMember(dto => dto.Options, opt => opt.MapFrom(src => src.Question.Options))
            .ForMember(dto => dto.WrittenAnswer, opt => opt.MapFrom(src => src.WrittenAnswer))
            .ForMember(dto => dto.SelectedOptionId, opt => opt.MapFrom(src => src.OptionId));

        CreateMap<Answer, AnswerDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Question.Instructions))
            .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => src.Question.Explanation))
            .ForMember(dest => dest.Feedback, opt => opt.MapFrom(src => src.Feedback))
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Question.Answer))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.Question.Mark))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Mark))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Question.Options.Select(o => new OptionDto
            {
                OptionText = o.OptionText,
                IsCorrect = o.IsCorrect,
                OptionNumber = o.OptionNumber,
                IsSelected = o.OptionId == src.OptionId
            })));

        CreateMap<Attempt, AttemptResultDto>()
            .ForMember(a => a.AttemptId, opt => opt.MapFrom(src => src.Id))
            .ForMember(a => a.QuizName, opt => opt.MapFrom(src => src.Quiz.Title))
            .ForMember(a => a.TotalScore, opt => opt.MapFrom(src => src.Quiz.Questions.Sum(q => q.Mark)))
            .ForMember(a => a.AchievedScore, opt => opt.MapFrom(src => src.Answers.Sum(a => a.Mark)))
            .ForMember(a => a.TimeSpentInSeconds, opt => opt.MapFrom(src => src.TimeSpent!.Value ));
    }
}