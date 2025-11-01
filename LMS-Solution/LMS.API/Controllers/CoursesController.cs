using LMS.Core.Commands.Courses.CreateCommands;
using LMS.Core.Commands.Courses.DeleteCommands;
using LMS.Core.Commands.Courses.UpdateCommands;
using LMS.Core.Queries.Courses.GetAllQueries;
using LMS.Core.Queries.Courses.GetByIdQueries;
using LMS.Shared.DTOs.Courses;
using LMS.Shared.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<IActionResult> Create(CreateCourseCommand command)
    {
        int id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAllCoursesDto>>> GetAll([FromQuery] GetAllCoursesQuery query)
    {
        var dto = await _mediator.Send(query);
        return dto;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCourseDto>> GetById(int id)
    {
        var dto = await _mediator.Send(new GetCourseByIdQuery(id));
        return dto;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCourseCommand(id));
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDetailsCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }
}