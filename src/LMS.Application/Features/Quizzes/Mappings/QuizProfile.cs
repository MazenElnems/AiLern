using AutoMapper;
using LMS.Application.Features.Quizzes.Commands.CreateQuiz;
using LMS.Application.Features.Quizzes.DTO;
using LMS.Domain.Entities.Quizzes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Mappings
{
    public class QuizProfile : Profile
    {
        public QuizProfile()
        {
            CreateMap<CreateQuizCommand, Quiz>();
            CreateMap<Quiz, QuizDto>();
        }
    }
}
