namespace LMS.Application.Contracts.Jobs;

public interface IConfirmUploadedFilesJob
{
    Task ExecuteAsync(List<string> Keys);
}
