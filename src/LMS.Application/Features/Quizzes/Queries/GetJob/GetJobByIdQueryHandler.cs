using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
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
    private readonly IUserContext _userContext;

    public GetJobByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<GetJobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUser().Id;

        var job = await _unitOfWork.QuestionGenerationJobs.GetAsync(j => j.Id == request.Id,
            includeProperties: [nameof(AIQuestionGenerationJob.Quiz)]);

        if (job == null)
            return DomainErrors.QuestionGenerationJob.NotFound(request.Id);

        var course = await _unitOfWork.Courses.GetByIdAsync(job.Quiz.CourseId);
        if (course == null)
            return Result<GetJobDto>.Failure(DomainErrors.Course.NotFound(job.Quiz.CourseId));
        if (course.InstructorId != userId)
            return Result<GetJobDto>.Failure(DomainErrors.Course.NotOwned);

        var dto = _mapper.Map<GetJobDto>(job);
        return Result<GetJobDto>.Success(dto);
    }
}
