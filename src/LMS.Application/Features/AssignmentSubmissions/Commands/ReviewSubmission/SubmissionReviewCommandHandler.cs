using LMS.Application.Common.Results;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Application.CurrentUser;
using LMS.Domain.Entities.Assignments;
using LMS.Domain.Errors;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LMS.Application.Features.AssignmentSubmissions.Commands.ReviewSubmission;

public class SubmissionReviewCommandHandler : IRequestHandler<SubmissionReviewCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _user;
    private readonly ILogger<SubmissionReviewCommandHandler> _logger;

    public SubmissionReviewCommandHandler(IUnitOfWork unitOfWork, IUserContext user, ILogger<SubmissionReviewCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _user = user;
        _logger = logger;
    }

    public async Task<Result> Handle(SubmissionReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = _user.GetCurrentUser().Id;
            var submission = await _unitOfWork.AssignmentSubmissions.GetAsync(a => a.Id == request.Id, includeProperties: [nameof(AssignmentSubmission.Assignment)]);
            if (submission == null)
            {
                return DomainErrors.AssignmentSubmission.NotFound(request.Id.ToString());
            }
            var course = await _unitOfWork.Courses.GetByIdAsync(submission.Assignment.CourseId);
            if (course.InstructorId != userId)
            {
                return DomainErrors.Course.NotOwned;
            }
            submission.Feedback = request.Feedback;
            await _unitOfWork.CommitAsync();
            return Result.Success();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while Review submission .");
            throw;
        }
        

    }
}
