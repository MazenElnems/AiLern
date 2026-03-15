using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetQuestionGenerationFiles;

public class GetQuestionGenerationFilesQueryHandler : IRequestHandler<GetQuestionGenerationFilesQuery, Result<List<QuestionGenerationFilesDto>>>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetQuestionGenerationFilesQueryHandler(IUserContext userContext, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _userContext = userContext;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<QuestionGenerationFilesDto>>> Handle(GetQuestionGenerationFilesQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var quiz = await _unitOfWork.Quizzes.GetAsync(q => q.Id == request.QuizId, 
            includeProperties: [nameof(Quiz.Course), nameof(Quiz.QuestionGenerationFiles)]);

        if (quiz == null)
            return DomainErrors.Quiz.NotFound(request.QuizId);

        if(quiz.Course.InstructorId != userId)
            return DomainErrors.Quiz.NotOwned;

        var questionGenerationFiles = quiz.QuestionGenerationFiles;

        var dto = _mapper.Map<List<QuestionGenerationFilesDto>>(questionGenerationFiles);

        return dto;
    }
}
