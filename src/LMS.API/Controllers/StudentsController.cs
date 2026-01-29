using LMS.Application.Commands.Students.CreateCommands;
using LMS.Application.Queries.Students.GetMyCoursesQuery;
using LMS.Domain.Constants;
using LMS.Domain.DTOs.Courses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    //[Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> Create(CreateStudentCommand command)
    {
        await _mediator.Send(command);
        return Created();
    }
    [HttpGet("my-courses")]
    public async Task<ActionResult<List<GetStudentCoursesDto>>> GetCourses([FromQuery]GetStudentCoursesQuery query)
    {
        var dto = await _mediator.Send(query);
        return dto;
    }
}
