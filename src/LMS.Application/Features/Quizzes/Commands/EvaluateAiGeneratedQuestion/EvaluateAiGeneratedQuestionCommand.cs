using LMS.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Commands.EvaluateAiGeneratedQuestion
{
    public class EvaluateAiGeneratedQuestionCommand: IRequest<Result>
    {
        [JsonIgnore]
        public Guid QuizId { get; set; }
        [JsonIgnore]
        public Guid QuestionId {get;set;}
        public bool? IsRelated { get; set; }
    }

}
