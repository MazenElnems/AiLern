using LMS.Core.Commands.Courses.ApproveCommands;
using LMS.Core.Commands.Courses.ApproveEntrollmentsCommands;
using LMS.Core.Commands.Courses.CourseEnrollmentsCommands;
using LMS.Core.Commands.Courses.CreateCommands;
using LMS.Core.Commands.Courses.DeleteCommands;
using LMS.Core.Commands.Courses.DeleteEnrollmentCommands;
using LMS.Core.Commands.Courses.RejectCommands;
using LMS.Core.Commands.Courses.RejectEnrollmentCommands;
using LMS.Core.Commands.Courses.UpdateCommands;
using LMS.Core.DTOs.Courses;
using LMS.Core.DTOs.Students;
using LMS.Core.Queries.Courses.GetAllQueries;
using LMS.Core.Queries.Courses.GetByIdQueries;
using LMS.Core.Queries.Courses.GetPendingQueries;
using LMS.Core.Queries.Courses.GetStudentsByCourseId;
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



    [HttpPut("{id}/reject")]
    public async Task<ActionResult<string>> Reject(int id,RejectCourseCommand command)
    {
        command.Id = id;
        var reason = await _mediator.Send(command);
        return reason;
    }

    [HttpGet("status")]
    public async Task<ActionResult<IEnumerable<GetCourseDto>>> GetByStatus([FromQuery] GetCoursesByStatusQuery query )
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

    [HttpPut("{id}/enrollments/{studentId}/approve")]
    public async Task<IActionResult> ApproveEnrollment(int id,int studentId)
    {
        await _mediator.Send(new ApproveEnrollmentCommand(id, studentId));
        return NoContent();
    }

    [HttpDelete("{id}/enrollments/{studentId}")]
    public async Task<IActionResult> DeleteEnrollment(int id,int studentId)
    {
        await _mediator.Send(new DeleteEnrollmentCommand(id, studentId));
        return NoContent();
    }

    [HttpPut("{id}/enrollments/{studentId}/reject")]
    public async Task<ActionResult<string>> RejectEnrollment(int id ,int studentId,RejectEnrollmentCommand command)
    {
        command.CourseId = id ;
        command.StudentId = studentId;
        var reason = await _mediator.Send(command);
        return reason;
    }

    [HttpGet("{id}/students")]
    public async Task<ActionResult<List<GetStudentsByCourseIdDto>>> GetStudentsByCourseId(int id)
    {
        var dto = await _mediator.Send(new GetStudentsByCourseIdQuery(id));
        return dto;
    }

    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> EnrollCourse(int id)
    {
        await _mediator.Send(new EnrollCourseCommand(id));
        return Created();
    }

    [HttpGet("available-courses")]
    public async Task<ActionResult<List<GetAvailableCoursesDto>>> GetAvailableCourses([FromQuery] GetAvailableCoursesQuery query)
    {
        var dto = await _mediator.Send(query);
        return dto;
    }

    [HttpGet("enrollment-requests")]
    public async Task<ActionResult<List<GetAllCoursesDto>>> GetCoursesWithEnrollmentRequests([FromQuery] GetWithEnrollmentRequestsQuery query)
    {

    }
}