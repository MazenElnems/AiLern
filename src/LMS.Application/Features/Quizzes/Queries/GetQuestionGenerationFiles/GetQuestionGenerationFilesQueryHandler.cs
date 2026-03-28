using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetQuestionGenerationFiles;

public class GetQuestionGenerationFilesQueryHandler : IRequestHandler<GetQuestionGenerationFilesQuery, Result<List<QuestionGenerationFilesDto>>>
{
    private readonly IPermissionService _permissionService;
    private readonly IUnitOfWork _unitOfWork;

    public GetQuestionGenerationFilesQueryHandler(IPermissionService permissionService, IUnitOfWork unitOfWork)
    {
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<QuestionGenerationFilesDto>>> Handle(GetQuestionGenerationFilesQuery request, CancellationToken cancellationToken)
    {
        var quizResult = await _permissionService.AuthorizeInstructorAccessToQuizAsync(request.QuizId);
        if (!quizResult.IsSuccess) return Result<List<QuestionGenerationFilesDto>>.Failure(quizResult.Error!);

        var questionGenerationFiles = (await _unitOfWork.QuestionGenerationFiles
            .FilterAsync(f => f.QuizId == request.QuizId)).ToList();

        var questionGenerationFilesIds = questionGenerationFiles.Select(x => x.Id);

        var sections = await _unitOfWork.Sections.FilterAsync(s => s.MaterialFiles.Any(m => questionGenerationFilesIds.Contains(m.Id)));

        var materialFiles = sections.SelectMany(s => s.MaterialFiles).Where(m => questionGenerationFilesIds.Contains(m.Id));

        var files = questionGenerationFiles.GroupJoin(materialFiles, qf => qf.Id, mf => mf.Id,
            (qf, mf) => new { File = qf, MFiles = mf })
            .SelectMany(x => x.MFiles.DefaultIfEmpty(),
            (qf, mf) => new { qf.File.Id, qf.File.FileName, mf?.SectionId, SectionName = mf?.Section.Title })
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
