using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Courses.Commands.ApproveEnrollment;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Commands.CreateEnrollment;
using LMS.Application.Features.Courses.Commands.DeleteCourse;
using LMS.Application.Features.Courses.Commands.DeleteEnrollment;
using LMS.Application.Features.Courses.Commands.RejectCourse;
using LMS.Application.Features.Courses.Commands.RejectEnrollment;
using LMS.Application.Features.Courses.Commands.UpdateCourse;
using LMS.Application.Features.Courses.Queries.GetAllCourses;
using LMS.Application.Features.Courses.Queries.GetAvailableCourses;
using LMS.Application.Features.Courses.Queries.GetById;
using LMS.Application.Features.Courses.Queries.GetCoursesByInstructorId;
using LMS.Application.Features.Courses.Queries.GetEnrolledStudents;
using LMS.Application.Features.Courses.Shared.DTO;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[SwaggerTag("Course management endpoints.")]
public class CoursesController : ApiBaseController
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Create course", Description = "Creates a new course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course created successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Create(CreateCourseCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get all courses", Description = "Retrieves all courses with pagination and filtering.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAll([FromQuery] GetAllCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get course by ID", Description = "Retrieves course details by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetById(int id)
    {
        var result = await _mediator.Send(new GetCourseByIdQuery(id));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete course", Description = "Deletes a course by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Delete(int id)
    {
        var result = await _mediator.Send(new DeleteCourseCommand(id));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update course", Description = "Updates course details by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course updated successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Update(int id, UpdateCourseDetailsCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/reject")]
    [SwaggerOperation(Summary = "Reject course", Description = "Rejects a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course rejected successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Reject(int id,RejectCourseCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/enrollments/{studentId}/approve")]
    [SwaggerOperation(Summary = "Approve enrollment", Description = "Approves a student's enrollment request.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment approved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> ApproveEnrollment(int id,int studentId)
    {
        var result = await _mediator.Send(new ApproveEnrollmentCommand(id, studentId));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/enrollments/{studentId}")]
    [SwaggerOperation(Summary = "Delete enrollment", Description = "Removes a student enrollment from a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeleteEnrollment(int id,int studentId)
    {
        var result = await _mediator.Send(new DeleteEnrollmentCommand(id, studentId));
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/enrollments/{studentId}/reject")]
    [SwaggerOperation(Summary = "Reject enrollment", Description = "Rejects a student's enrollment request.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment rejected successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> RejectEnrollment(int id ,int studentId,RejectEnrollmentCommand command)
    {
        command.CourseId = id ;
        command.StudentId = studentId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/students")]
    [SwaggerOperation(Summary = "Get enrolled students", Description = "Lists students enrolled in a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Students retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetStudentsByCourseId(int id)
    {
        var query = new GetStudentsByCourseIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/enroll")]
    [SwaggerOperation(Summary = "Enroll in course", Description = "Creates an enrollment request for the current student.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment request created.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> EnrollCourse(int id)
    {
        var result = await _mediator.Send(new EnrollCourseCommand(id));
        return HandleResponse(this, result);
    }

    [HttpGet("available-courses")]
    [SwaggerOperation(Summary = "Get available courses", Description = "Retrieves courses available for enrollment.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAvailableCourses([FromQuery] GetAvailableCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpGet("instructors/{id}")]
    [Authorize]
    [SwaggerOperation(
    Summary = "Get courses by instructor id", Description = "Retrieves approved courses for a specific instructor.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Instructor not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCoursesByInstructorId(int id)
    {
        var query = new GetCoursesByInstructorIdQuery(id);

        var result = await _mediator.Send(query);

        return HandleResponse(this, result);
    }
}