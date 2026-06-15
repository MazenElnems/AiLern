using AutoMapper;
using LMS.Application.Features.Attempts.Shared.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Attempts.Shared.Mappings;

public class AttemptProfile : Profile
{
    public AttemptProfile()
    {
        CreateMap<Attempt, AttemptResultDto>()
            .ForMember(dest => dest.Answers, opt => opt.MapFrom(src => src.Answers.OrderBy(a => a.Question.Order)));

        CreateMap<Option, AttemptOptionDto>()
            .ForMember(dto => dto.Option, opt => opt.MapFrom(src => src.OptionText))
            .ForMember(dto => dto.Order, opt => opt.MapFrom(src => src.OptionNumber));

        CreateMap<Answer, AttemptQuestionDto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Question.Id))
            .ForMember(dto => dto.Question, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dto => dto.Type, opt => opt.MapFrom(src => src.Question.Type))
            .ForMember(dto => dto.Instructions, opt => opt.MapFrom(src => src.Question.Instructions))
            .ForMember(dto => dto.Options, opt => opt.MapFrom(src => src.Question.Options))
            .ForMember(dto => dto.Order, opt => opt.MapFrom(src => src.Question.Order))
            .ForMember(dto => dto.WrittenAnswer, opt => opt.MapFrom(src => src.WrittenAnswer))
            .ForMember(dto => dto.Mark, opt => opt.MapFrom(src => src.Question.Mark))
            .ForMember(dto => dto.SelectedOptionId, opt => opt.MapFrom(src => src.OptionId))
            .ForMember(dto => dto.ShuffledOptionIds, opt => opt.MapFrom(src => src.ShuffledOptionIds));

        CreateMap<Answer, AnswerDto>()
            .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question.QuestionText))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Question.Type))
            .ForMember(dest => dest.Instructions, opt => opt.MapFrom(src => src.Question.Instructions))
            .ForMember(dest => dest.Explanation, opt => opt.MapFrom(src => src.Question.Explanation))
            .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Question.Order))
            .ForMember(dest => dest.MaxScore, opt => opt.MapFrom(src => src.Question.Mark))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Mark))
            .ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.WrittenAnswer))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Question.Options.OrderBy(o => o.OptionNumber)
                .Select(o => new OptionAnswerDto
                {
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect,
                    Order = o.OptionNumber,
                    IsSelected = o.OptionId == src.OptionId
                })));

        CreateMap<Attempt, AttemptResultDto>()
            .ForMember(a => a.AttemptId, opt => opt.MapFrom(src => src.Id))
            .ForMember(a => a.QuizTitle, opt => opt.MapFrom(src => src.Quiz.Title))
            .ForMember(a => a.TotalScore, opt => opt.MapFrom(src => src.Quiz.Questions.Sum(q => q.Mark)))
            .ForMember(a => a.Score, opt => opt.MapFrom(src => src.Answers.Sum(a => a.Mark)))
            .ForMember(a => a.TimeSpent, opt => opt.MapFrom(src => src.TimeSpent!.Value))
            .ForMember(a => a.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(a=> a.WeakTopics, opt => opt.MapFrom(src => src.WeakTopics.Select(wt => wt.Topic).ToList()))
            .ForMember(a => a.StudentName, opt => opt.MapFrom(src => src.Student.UserName));
    }
}