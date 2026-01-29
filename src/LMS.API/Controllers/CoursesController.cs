using LMS.Application.Commands.Courses.ApproveEntrollmentsCommands;
using LMS.Application.Commands.Courses.CourseEnrollmentsCommands;
using LMS.Application.Queries.Courses.GetAvailableCoursesQueries;
using LMS.Application.Commands.Courses.DeleteEnrollmentCommands;
using LMS.Application.Commands.Courses.RejectEnrollmentCommands;
using LMS.Application.Queries.Courses.GetStudentsByCourseId;
using LMS.Application.Commands.Courses.CreateCommands;
using LMS.Application.Commands.Courses.DeleteCommands;
using LMS.Application.Commands.Courses.RejectCommands;
using LMS.Application.Commands.Courses.UpdateCommands;
using LMS.Application.Queries.Courses.GetByIdQueries;
using LMS.Application.Queries.Courses.GetAllQueries;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using LMS.Application.Queries.Courses.GetEnrollmentRequestsQueries;
using LMS.Domain.DTOs.Courses;
using LMS.Domain.DTOs.Students;
using LMS.Domain.DTOs;


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
    public async Task<ActionResult<PaginationResult<GetAllCoursesDto>>> GetAll([FromQuery] GetAllCoursesQuery query)
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
        var query = new GetStudentsByCourseIdQuery { Id = id };
        var dto = await _mediator.Send(query);
        return dto;
    }

    [HttpPost("{id}/enroll")]
    public async Task<IActionResult> EnrollCourse(int id)
    {
        await _mediator.Send(new EnrollCourseCommand(id));
        return Created();
    }

    [HttpGet("available-courses")]
    public async Task<ActionResult<PaginationResult<GetAvailableCoursesDto>>> GetAvailableCourses([FromQuery] GetAvailableCoursesQuery query)
    {
        var dto = await _mediator.Send(query);
        return dto;
    }

    [HttpGet("{id}/enrollment-requests")]
    public async Task<ActionResult<List<GetEnrollmentRequestsDto>>> GetEnrollmentRequests(int id,[FromQuery] GetEnrollmentRequestsQuery query)
    {
        query.CourseId = id;
        var dto = await _mediator.Send(query);
        return dto;
    }
}