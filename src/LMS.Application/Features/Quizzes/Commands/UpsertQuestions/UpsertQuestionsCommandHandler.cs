using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using Microsoft.EntityFrameworkCore; 
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using MediatR;
using LMS.Domain.Enums;

namespace LMS.Application.Features.Quizzes.Commands.UpsertQuestions;

public class UpsertQuestionsCommandHandler : IRequestHandler<UpsertQuestionsCommand, Result>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertQuestionsCommandHandler(IUnitOfWork unitOfWork, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result> Handle(UpsertQuestionsCommand request, CancellationToken cancellationToken)
    {
        var instructorId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.Query
            .Include(q => q.Course)
            .Include(q => q.Questions)
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(q => q.Id == request.QuizId, cancellationToken);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if (quiz.Course.InstructorId != instructorId)
            return DomainErrors.Quiz.NotOwned;

        // cannot update questions after the quiz has started
        if (quiz.AvailableFrom < DateTime.UtcNow && quiz.Status == QuizStatus.Published)
            return DomainErrors.Quiz.UpdateQuestionsAfterQuizStarted;

        // cannot remove questions from a published quiz
        if(quiz.Status == QuizStatus.Published && request.Questions.Count == 0)
            return DomainErrors.Quiz.CannotPublishEmptyQuiz;

        var questions = quiz.Questions;

        var requestedQuestionIds = request.Questions.Select(q => q.Id);

        questions.RemoveAll(q => !requestedQuestionIds.Contains(q.Id));

        int order = 1;
        foreach(var question in request.Questions)
        {
            // UpdatedQuestion
            if (question.Id.HasValue)
            {
                var updatedQuestion = questions.FirstOrDefault(q => q.Id == question.Id);
                if (updatedQuestion != null)    
                {
                    updatedQuestion.QuestionText = question.QuestionText;
                    updatedQuestion.Mark = question.Mark;
                    updatedQuestion.Explanation = question.Explanation;
                    updatedQuestion.Instructions = question.Instructions;
                    updatedQuestion.Order = order;

                    // Remove deleted options
                    updatedQuestion.Options.RemoveAll(o => !question.Options.Select(opt => opt.OptionId).Contains(o.OptionId));

                    int optionOrder = 1;
                    question.Options.ForEach(o =>
                    {
                        // UpdatedOption
                        if (o.OptionId.HasValue)
                        {
                            var updatedOption = updatedQuestion.Options.FirstOrDefault(opt => opt.OptionId == o.OptionId);

                            if (updatedOption != null)
                            {
                                updatedOption.OptionText = o.OptionText;
                                updatedOption.IsCorrect = o.IsCorrect;
                                updatedOption.OptionNumber = optionOrder;
                            }
                        }
                        // New Added Option
                        else
                        {
                            var addedOption = new Option
                            {
                                OptionText = o.OptionText,
                                IsCorrect = o.IsCorrect,
                                OptionNumber = optionOrder
                            };

                            updatedQuestion.Options.Add(addedOption);
                        }

                        optionOrder++;
                    });
                }
            }
            // New Added Question
            else
            {
                var addedQuestion = new Question
                {
                    QuestionText = question.QuestionText,
                    Type = question.QuestionType,
                    Instructions = question.Instructions,
                    Explanation = question.Explanation,
                    Mark = question.Mark,
                    Order = order,
                    Options = question.Options.Select((o, i) => new Option
                    {
                        OptionText = o.OptionText,
                        OptionNumber = i + 1,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };

                questions.Add(addedQuestion);
            }

            order++;
        }
        
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result.Success("Quiz questions updated Successfully.");
    }
}

