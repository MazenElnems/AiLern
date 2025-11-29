using LMS.Core.Commands.Courses.ApproveCommands;
using LMS.Core.Commands.Courses.CreateCommands;
using LMS.Core.Commands.Courses.DeleteCommands;
using LMS.Core.Commands.Courses.RejectCommands;
using LMS.Core.Commands.Courses.RejectEnrollmentCommands;
using LMS.Core.Commands.Courses.UpdateCommands;
using LMS.Core.DTOs.Courses;
using LMS.Core.Queries.Courses.GetAllQueries;
using LMS.Core.Queries.Courses.GetApprovedQueries;
using LMS.Core.Queries.Courses.GetByIdQueries;
using LMS.Core.Queries.Courses.GetPendingQueries;
using LMS.Core.Queries.Courses.GetStudentsByCourseId;
using LMS.Domin.Entities;
using MediatR;
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

    [HttpGet("approved-courses")]
    public async Task<ActionResult<IEnumerable<GetApprovedCoursesDto>>> GetApproved([FromQuery] GetApprovedCoursesQuery command)
    {
        var dto = await _mediator.Send(command);
        return dto;
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult<string>> Reject(int id,RejectCourseCommand command)
    {
        command.Id = id;
        var reason = await _mediator.Send(command);
        return reason;
    }

    [HttpGet("pending-courses")]
    public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetPending([FromQuery] GetPendingCoursesQuery query)
    {
        var dto = await _mediator.Send(query);
        return dto;

    }
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        await _mediator.Send(new ApproveCourseCommand(id));
        return NoContent();
    }
    [HttpPut("{id}/enrollments/{studentId}/reject")]
    public async Task<ActionResult<string>> RejectEnrollment(int id ,int studentId,RejectEnrollmentCommand command)
    {
        command.CourseId = id ;
        command.StudentId= studentId;
        var reason = await _mediator.Send(command);
        return reason;
    }
    [HttpGet("{id}/students")]
    public async Task<ActionResult<List<string>>> GetStudentsByCourseId(int id)
    {
        var en = await _mediator.Send(new GetStudentsByCourseIdQuery(id));
        return en;
    }
}