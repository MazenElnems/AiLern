using LMS.Application.Contracts.Jobs;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Jobs;

public class AutoSubmitAttemptJob : IAutoSubmitAttemptJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalculateStudentScoreJob _calculateStudentScoreJob;
    private readonly ILogger<AutoSubmitAttemptJob> _logger;

    public AutoSubmitAttemptJob(IUnitOfWork unitOfWork, ILogger<AutoSubmitAttemptJob> logger, ICalculateStudentScoreJob calculateStudentScoreJob)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _calculateStudentScoreJob = calculateStudentScoreJob;
    }

    public async Task ExecuteAsync(Guid attemptId, CancellationToken cancellationToken)
    {
        var attempt = await _unitOfWork.Attempts.GetByIdAsync(attemptId);

        if (attempt == null)
            return;

        if (attempt.Status != AttemptStatus.InProgress)
            return;

        _logger.LogInformation("Auto-submitting attempt {@Attempt}", attempt);

        attempt.Submit();
        await _unitOfWork.CommitAsync();
        
        await _calculateStudentScoreJob.ExecuteAsync(attemptId, cancellationToken);
    }
}

