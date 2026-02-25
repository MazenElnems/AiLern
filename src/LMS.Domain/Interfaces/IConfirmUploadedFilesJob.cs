namespace LMS.Domain.Interfaces;

public interface IConfirmUploadedFilesJob
{
    Task ExecuteAsync(List<string> Keys);
}
