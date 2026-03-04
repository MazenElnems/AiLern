using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Commands.CreateQuiz
{
    public class CreateQuizCommand : IRequest<Result<GetAllQuizDto>>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableUntil { get; set; }
        public int MaximumAttempts { get; set; }
        public bool ShowCorrectAnswersAfterClose { get; set; }
        public double TotalPoints { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleOptions { get; set; }
        public int CourseId { get; set; }
    }
}
