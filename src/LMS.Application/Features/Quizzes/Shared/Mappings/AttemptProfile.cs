using AutoMapper;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Quizzes.Shared.Mappings;

public class AttemptProfile : Profile
{
    public AttemptProfile()
    {
        CreateMap<Attempt, GetSubmissionsByQuizIdDto>()
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => src.Answers.Sum(a => a.Mark)))
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student.FullName));
    }
}
