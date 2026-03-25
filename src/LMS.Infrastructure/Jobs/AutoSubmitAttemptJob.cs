using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Jobs;

public class AutoSubmitAttemptJob : IAutoSubmitAttemptJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AutoSubmitAttemptJob> _logger;

    public AutoSubmitAttemptJob(IUnitOfWork unitOfWork, ILogger<AutoSubmitAttemptJob> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(Guid attemptId)
    {
        var attempt = await _unitOfWork.Attempts.GetByIdAsync(attemptId);

        if (attempt == null)
            return;

        if (attempt.Status != AttemptStatus.InProgress)
            return;

        _logger.LogInformation("Auto-submitting attempt {AttemptId}", attemptId);

        attempt.Submit();
        await _unitOfWork.CommitAsync();
    }
}
