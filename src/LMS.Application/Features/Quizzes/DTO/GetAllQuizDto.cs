using LMS.Domain.Entities.Quizzes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.DTO
{
    public class GetAllQuizDto
    {
        public Guid Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableUntil { get; set; }
        public bool IsPublished { get; set; }
        public int MaximumAttempts { get; set; }
        public bool ShowCorrectAnswersAfterClose { get; set; }
        public double TotalPoints { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleOptions { get; set; }
        public DateTime CreatedAt { get; set; }

    }

}

