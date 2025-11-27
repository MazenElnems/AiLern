namespace LMS.Domin.Contracts;

public interface IMailSender
{
    Task SendAsync(string to, string subject, string body);
}
