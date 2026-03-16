using AutoMapper;
using LMS.Application.Common.Results.Generic;
using LMS.Application.CurrentUser;
using LMS.Application.Features.Quizzes.Shared.DTO;
using LMS.Domain.Entities.Courses;
using LMS.Domain.Entities.Quizzes;
using LMS.Domain.Errors;
using LMS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.Quizzes.Queries.GetJob;

public class GetJobByIdQueryHandler : IRequestHandler<GetJobByIdQuery, Result<GetJobDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetJobByIdQueryHandler> _logger;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public GetJobByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetJobByIdQueryHandler> logger, IMapper mapper, IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<GetJobDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _userContext.GetCurrentUser().Id;

            var job = await _unitOfWork.QuestionGenerationJobs.GetAsync(j => j.Id == request.Id, includeProperties: [nameof(AIQuestionGenerationJob.Quiz)]);

            if (job == null)
                return DomainErrors.QuestionGenerationJob.NotFound(request.Id);

            var isInstructor = await _unitOfWork.Courses.AnyAsync(c => c.Id == job.Quiz.CourseId && c.InstructorId == userId);
            if (!isInstructor)
                return DomainErrors.Course.NotOwned;

            var dto = _mapper.Map<GetJobDto>(job);

            return Result<GetJobDto>.Success(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving job.");
            throw;
        }
    }
}
