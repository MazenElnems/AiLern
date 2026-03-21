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

        var questionGenerationFilesIds = questionGenerationFiles.Select(x => x.Id);

        var sections = await _unitOfWork.Sections.FilterAsync(s => s.MaterialFiles.Any(m=> questionGenerationFilesIds.Contains(m.Id)));

        var materialFiles = sections.SelectMany(s => s.MaterialFiles).Where(m => questionGenerationFilesIds.Contains(m.Id));

        var files = questionGenerationFiles.GroupJoin(materialFiles, qf => qf.Id, mf => mf.Id, 
            (qf,mf) => new { File = qf , MFiles=mf })
            .SelectMany(x => x.MFiles.DefaultIfEmpty(), 
            (qf, mf) => new { qf.File.Id, qf.File.FileName, mf?.SectionId,SectionName = mf?.Section.Title })
            .ToList();

        var dto = files
            .GroupBy(f => new { f.SectionId, f.SectionName })
            .Select(g => new QuestionGenerationFilesDto
            {
                SectionId = g.Key.SectionId,
                SectionName = g.Key.SectionName,
                Files = g.Select(x => new FileDto
                {
                    Id = x.Id,
                    FileName = x.FileName
                }).ToList()
            })
            .ToList();

        return dto;
    }
}
