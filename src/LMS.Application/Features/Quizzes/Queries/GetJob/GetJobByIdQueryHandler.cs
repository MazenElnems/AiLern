using AutoMapper;
using LMS.Application.Common.Interfaces;
using LMS.Application.Common.Results.Generic;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;

namespace LMS.Application.Features.Quizzes.Queries.GetJob;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<GetJobDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPermissionService _permissionService;

    public GetJobByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _permissionService = permissionService;
    }

    public async Task<Result<GetJobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _unitOfWork.QuestionGenerationJobs.GetAsync(j => j.Id == request.Id,
            includeProperties: [nameof(AIQuestionGenerationJob.Quiz)]);

        if (job == null)
            return DomainErrors.QuestionGenerationJob.NotFound(request.Id);

        var courseResult = await _permissionService.AuthorizeInstructorAccessToCourseAsync(job.Quiz.CourseId);
        if (!courseResult.IsSuccess) return Result<GetJobDto>.Failure(courseResult.Error!);

        var dto = _mapper.Map<GetJobDto>(job);
        return Result<GetJobDto>.Success(dto);
    }
}
