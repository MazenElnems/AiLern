using MediatR;

namespace LMS.Application.Commands.Admins.CreateAdminCommands;

public class CreateAdminCommand : IRequest<int>
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}
