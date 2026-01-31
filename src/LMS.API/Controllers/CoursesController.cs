using LMS.API.Common.Responses;
using LMS.API.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Commands.UpdateCourse;
using LMS.Application.Features.Courses.Commands.RejectCourse;
using LMS.Application.Features.Courses.Commands.RejectEnrollment;
using LMS.Application.Features.Courses.Queries.GetAllCourses;
using LMS.Application.Features.Courses.Queries.GetAvailableCourses;
using LMS.Application.Features.Courses.Queries.GetEnrollmentRequests;
using LMS.Application.Features.Courses.Queries.GetById;
using LMS.Application.Features.Courses.Commands.DeleteCourse;
using LMS.Application.Features.Courses.Commands.ApproveEnrollment;
using LMS.Application.Features.Courses.Commands.DeleteEnrollment;
using LMS.Application.Features.Courses.Queries.GetEnrolledStudents;
using LMS.Application.Features.Courses.Commands.CreateEnrollment;


namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ApiBaseController
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Create(CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] GetAllCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> Update(int id, UpdateCourseDetailsCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/reject")]
    public async Task<ActionResult<ApiResponse>> Reject(int id,RejectCourseCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/enrollments/{studentId}/approve")]
    public async Task<ActionResult<ApiResponse>> ApproveEnrollment(int id,int studentId)
    {
        var result = await _mediator.Send(new ApproveEnrollmentCommand(id, studentId));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/enrollments/{studentId}")]
    public async Task<ActionResult<ApiResponse>> DeleteEnrollment(int id,int studentId)
    {
        var result = await _mediator.Send(new DeleteEnrollmentCommand(id, studentId));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/enrollments/{studentId}/reject")]
    public async Task<ActionResult<ApiResponse>> RejectEnrollment(int id ,int studentId,RejectEnrollmentCommand command)
    {
        command.CourseId = id ;
        command.StudentId = studentId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/students")]
    public async Task<ActionResult<ApiResponse>> GetStudentsByCourseId(int id)
    {
        var query = new GetStudentsByCourseIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/enroll")]
    public async Task<ActionResult<ApiResponse>> EnrollCourse(int id)
    {
        var result = await _mediator.Send(new EnrollCourseCommand(id));
        return HandleResponse(this, result);
    }

    [HttpGet("available-courses")]
    public async Task<ActionResult<ApiResponse>> GetAvailableCourses([FromQuery] GetAvailableCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/enrollment-requests")]
    public async Task<ActionResult<ApiResponse>> GetEnrollmentRequests(int id,[FromQuery] GetEnrollmentRequestsQuery query)
    {
        query.CourseId = id;
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }
}