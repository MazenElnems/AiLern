using LMS.Core.Commands.Students.CreateCommands;
using LMS.Core.Queries.Students.GetMyCoursesQuery;
using LMS.Domin.DTOs.Courses;
using MediatR;
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
