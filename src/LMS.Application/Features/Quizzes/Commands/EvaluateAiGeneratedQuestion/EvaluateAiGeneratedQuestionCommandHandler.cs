using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Application.Features.Quizzes.Commands.EvaluateAiGeneratedQuestion
{
    public class EvaluateAiGeneratedQuestionCommandHandler : IRequestHandler<EvaluateAiGeneratedQuestionCommand, Result>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContext _userContext;

        public EvaluateAiGeneratedQuestionCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _userContext = userContext;
        }
        public async Task<Result> Handle(EvaluateAiGeneratedQuestionCommand request, CancellationToken cancellationToken)
        {
            var instructorId = _userContext.GetCurrentUser().Id;

            var question = await _unitOfWork.Questions.Query
                .Include(q => q.Quiz)
                .ThenInclude(qz => qz.Course)
                .FirstOrDefaultAsync(
                    q => q.Id == request.QuestionId && q.QuizId == request.QuizId,
                    cancellationToken);

            if (question == null)
                return DomainErrors.QuizQuestion.NotFound(request.QuestionId);

            if (question.Quiz.Course.InstructorId != instructorId)
                return DomainErrors.Quiz.NotOwned;

            if (!question.IsAIGenerated || question.IsAccepted == true)
                return DomainErrors.QuizQuestion.NotPendingAi;

            question.IsRelated = request.IsRelated;
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Success("Question evaluated.");
        }
    }
}
