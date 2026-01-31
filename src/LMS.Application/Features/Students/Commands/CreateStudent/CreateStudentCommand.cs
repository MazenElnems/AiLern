using LMS.Application.Common.Results.Generic;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LMS.Application.Features.Students.Commands.CreateStudent;

public class CreateStudentCommand : IRequest<Result<int>>
{
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public int StudentId { get; set; }
}
