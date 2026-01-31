using LMS.Application.Common.Results.Generic;
using MediatR;

namespace LMS.Application.Features.Instructors.Commands.CreateInstructor;

public class CreateInstructorCommand : IRequest<Result<int>>
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
}
