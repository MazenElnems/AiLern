using LMS.Domain.Enums;
using LMS.Domain.Interfaces;
using LMS.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Jobs;

public  class ConfirmUploadedFilesJob : IConfirmUploadedFilesJob
{
    private readonly IWasabiService _wasabiService;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmUploadedFilesJob(IWasabiService wasabiService, IUnitOfWork unitOfWork)
    {
        _wasabiService = wasabiService;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(List<string> Keys)
    {
        var files = await _unitOfWork.MaterialFiles
            .TrackedQuery
            .Where(f => Keys.Contains(f.StoragePath))
            .ToListAsync();
        foreach (var file in files)
        {
            var exists = await _wasabiService.FileExists(file.StoragePath);

            file.UploadStatus = exists
                ? UploadStatus.Completed
                : UploadStatus.Failed;


        }
        await _unitOfWork.CommitAsync();

    }
}
