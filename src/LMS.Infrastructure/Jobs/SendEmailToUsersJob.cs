using LMS.Application.Contracts.Jobs;
using LMS.Application.Contracts.Services;
using LMS.Application.Contracts.UnitOfWork;
using LMS.Domain.Models.Notification;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Jobs;

public class SendEmailToUsersJob : ISendEmailToUsersJob
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;

    public SendEmailToUsersJob(IUnitOfWork unitOfWork, IEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
    }
    public async Task ExecuteAsync(IEnumerable<int> UsersId, NotificationEmailModel notificationEmailModel)
    {
        var users = await _unitOfWork.Users.Query.Where(u => UsersId.Contains(u.Id))
            .Select(u =>new { u.Email, u.FullName }).ToListAsync();

        var tasks = users.Select(user =>
            _emailSender.SendNotificationEmailWithUrlAsync(user.Email!, user.FullName, notificationEmailModel)
        );

        await Task.WhenAll(tasks);

    }
}
