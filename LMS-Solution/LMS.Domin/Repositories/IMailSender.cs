namespace LMS.Domin.Repositories;

public interface IMailSender
{
    Task SendAsync(string to, string subject, string body);
}
