using AutoMapper;
using LMS.Application.Features.Quizzes.DTO;
using LMS.Domain.Entities.Quizzes;

namespace LMS.Application.Features.Assignments.Mappings;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        CreateMap<Quiz, QuizDto>();
    }
}
