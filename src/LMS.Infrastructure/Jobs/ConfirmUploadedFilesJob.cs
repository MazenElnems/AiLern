using LMS.Application.Contracts.ExternalServices;
using LMS.Application.Contracts.Jobs;
using LMS.Domain.Enums;
using LMS.Domain.Repositories;

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
        var sections = await _unitOfWork.Sections
            .FilterAsync(s => s.MaterialFiles.Any(f => Keys.Contains(f.StoragePath)));

        var files = sections
            .ToList()
            .SelectMany(s => s.MaterialFiles);

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
