using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Courses.Commands.ConfirmAIResources;
using LMS.Application.Features.Courses.Commands.CreateCourse;
using LMS.Application.Features.Courses.Commands.CreateEnrollment;
using LMS.Application.Features.Courses.Commands.DeleteAIResources;
using LMS.Application.Features.Courses.Commands.DeleteCourse;
using LMS.Application.Features.Courses.Commands.DeleteEnrollment;
using LMS.Application.Features.Courses.Commands.UpdateCourse;
using LMS.Application.Features.Courses.Commands.UploadAIResources;
using LMS.Application.Features.Courses.Queries.GetAIResources;
using LMS.Application.Features.Courses.Commands.UpdateProgress;
using LMS.Application.Features.Courses.Queries.GetAllCourses;
using LMS.Application.Features.Courses.Queries.GetById;
using LMS.Application.Features.Courses.Queries.GetCoursesByInstructorId;
using LMS.Application.Features.Courses.Queries.GetEnrolledStudents;
using LMS.Application.Features.Courses.Queries.GetMyLearning;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using LMS.Application.Features.Courses.Queries.GetAIResourceStatus;
using LMS.Application.Features.CourseDiscussions.Commands.CourseDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.VoteDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.DownVoteDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.UpdateDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.DeleteDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.PinDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.UnPinDiscussion;
using LMS.Application.Features.CourseDiscussions.Commands.AnswerDiscussion;
using LMS.Application.Features.CourseDiscussions.Queries.GetDiscussions;

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

    [HttpPost("course/{courseId}/discussions")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Create discussion", Description = "Creates a new discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion created successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Create(int courseId, CreateDiscussionCommand command)
    {
        command.CourseId = courseId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [HttpPost("course/{courseId}/discussions/{discussionId}/up_vote")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Upvote discussion", Description = "Upvotes an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion upvoted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> UpVote(int courseId, Guid discussionId)
    {
        var result = await _mediator.Send(new UpVoteDiscussionCommand
        {
            CourseId = courseId,
            DiscussionId = discussionId
        });
        return HandleResponse(this, result);
    }

    [HttpDelete("course/{courseId}/discussions/{discussionId}/down_vote")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Downvote discussion", Description = "Downvotes an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion downvoted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DownVote(int courseId, Guid discussionId)
    {
        var result = await _mediator.Send(new DownVoteDiscussionCommand
        {
            CourseId = courseId,
            DiscussionId = discussionId
        });
        return HandleResponse(this, result);
    }

    [HttpPut("course/{courseId}/discussions/{discussionId}/update")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Update discussion", Description = "Updates an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion updated successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Update(int courseId, Guid discussionId, UpdateDiscussionCommand command)
    {
        command.CourseId = courseId;
        command.DiscussionId = discussionId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("course/{courseId}/discussions/{discussionId}")]
    [Authorize(Roles = UserRoles.Student + "," + UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Delete discussion", Description = "Deletes an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Delete(int courseId, Guid discussionId)
    {
        var result = await _mediator.Send(new DeleteDiscussionCommand
        {
            CourseId = courseId,
            DiscussionId = discussionId
        });
        return HandleResponse(this, result);
    }

    [HttpPut("course/{courseId}/discussions/{discussionId}/pin")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Pin discussion", Description = "Pins an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion pinned successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Pin(int courseId, Guid discussionId)
    {
        var result = await _mediator.Send(new PinDiscussionCommand
        {
            CourseId = courseId,
            DiscussionId = discussionId
        });
        return HandleResponse(this, result);
    }

    [HttpPut("course/{courseId}/discussions/{discussionId}/un_pin")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Unpin discussion", Description = "Unpins an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion unpinned successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> UnPin(int courseId, Guid discussionId)
    {
        var result = await _mediator.Send(new UnPinDiscussionCommand
        {
            CourseId = courseId,
            DiscussionId = discussionId
        });
        return HandleResponse(this, result);
    }

    [HttpPut("course/{courseId}/discussions/{discussionId}/answer")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Answer discussion", Description = "Answers an existing discussion for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussion answered successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Answer(int courseId, Guid discussionId,AnswerDiscussionCommand command)
    {
        command.CourseId = courseId;
        command.DiscussionId = discussionId;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [HttpGet("course/{courseId}/discussions")]
    [Authorize(Roles = UserRoles.Instructor + "," + UserRoles.Student)]
    [SwaggerOperation(Summary = "Get discussions for a course", Description = "Retrieves all discussions for a specific course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Discussions retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetDiscussions(int courseId)
    {
        var result = await _mediator.Send(new GetDiscussionsCommand { CourseId = courseId });
        return HandleResponse(this, result);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get all courses", Description = "Retrieves all courses with pagination and filtering.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid query parameters.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAll(int pageNo = 1, int pageSize = 10)
    {
        var result = await _mediator.Send(new GetAllCoursesQuery(pageNo, pageSize));
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

    [HttpGet("{id}/details")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Get course details by ID", Description = "Retrieves course details by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Course retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCourseDetailsById(int id)
    {
        var result = await _mediator.Send(new GetCourseDetailsByIdQuery(id));
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
    [HttpPut("{id}/ai-resources/confirm")]
    [SwaggerOperation(Summary = "Confirm AI resources", Description = "Confirms AI resources for a course by ID.")]
    [SwaggerResponse(StatusCodes.Status200OK, "AI resources confirmed successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> Update(int id, ConfirmAIResourcesCommand command)
    {
        command.CourseId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/enrollments/{studentId}")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Delete enrollment", Description = "Removes a student enrollment from a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeleteEnrollment(int id, int studentId)
    {
        var result = await _mediator.Send(new DeleteEnrollmentCommand(id, studentId));
        return HandleResponse(this, result);
    }

    [HttpDelete("{id}/ai-resources/{resourceId}/delete")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Delete enrollment", Description = "Removes a student enrollment from a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment deleted successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course or student not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> DeleteEnrollment(int id,Guid resourceId)
    {

        var result = await _mediator.Send(new DeleteAIResourcesCommand { CourseId = id , AiResourceId = resourceId});
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/students")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Get enrolled students", Description = "Lists students enrolled in a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Students retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetEnrolledStudents(int id, int pageNo = 1, int pageSize = 10, string searchString = "")
    {
        var result = await _mediator.Send(new GetEnrolledStudentsQuery(id, pageNo, pageSize, searchString));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/ai-resources")]
    [SwaggerOperation(Summary = "Get AI resources", Description = "Lists AI resources for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "AI resources retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetAIResources(int id)    
    {

        var result = await _mediator.Send(new GetAIResourcesCommand { CourseId = id });
        return HandleResponse(this, result);
    }

    [HttpPost("enroll")]
    [Authorize(Roles = UserRoles.Admin)]
    [SwaggerOperation(Summary = "Enroll in course", Description = "Creates an enrollment request.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Enrollment request created.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> EnrollStudent(int studentId, int courseId)
    {
        var result = await _mediator.Send(new EnrollCourseCommand(studentId, courseId));
        return HandleResponse(this, result);
    }

    [HttpPost("{id}/ai-resources")]
    [Authorize(Roles = UserRoles.Instructor)]
    [SwaggerOperation(Summary = "Upload AI resources", Description = "Uploads AI resources for a course.")]
    [SwaggerResponse(StatusCodes.Status200OK, "AI resources uploaded successfully   .", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Course not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> UploadAIResources(int id ,UploadAIResourcesCommand command)
    {
        
        command.CourseId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("instructors/{id}")]
    [Authorize]
    [SwaggerOperation(Summary = "Get courses by instructor id", Description = "Retrieves approved courses for a specific instructor.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Courses retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Instructor not found.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetCoursesByInstructorId(int id)
    {
        var query = new GetCoursesByInstructorIdQuery(id);
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }

    [HttpPut("{id}/progress")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<ActionResult<ApiResponse>> UpdateProgress(int id, UpdateStudentCourseProgressCommand command)
    {
        command.CourseId = id;
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }

    [HttpGet("my-learning")]
    [Authorize(Roles = UserRoles.Student)]
    [SwaggerOperation(Summary = "Get my learning", Description = "Returns the current student's enrolled courses with progress, ordered by last progress update.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Learning items retrieved successfully.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Unauthorized.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "Forbidden.", typeof(ApiResponse))]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Server error.", typeof(ApiResponse))]
    public async Task<ActionResult<ApiResponse>> GetMyLearning(int pageNo = 1, int pageSize = 5)
    {
        var result = await _mediator.Send(new GetMyLearningQuery(pageNo, pageSize));
        return HandleResponse(this, result);
    }

    [HttpGet("{id}/ai-status")]
    [Authorize(Roles = UserRoles.Instructor)]
    public async Task<ActionResult<ApiResponse>> GetAIStatus(int id)
    {
        var result = await _mediator.Send(new GetAIResourceStatusQuery (id));
        return HandleResponse(this, result);
    }
}
