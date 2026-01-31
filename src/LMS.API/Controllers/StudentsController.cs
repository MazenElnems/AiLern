using LMS.API.Controllers.Common;
using LMS.API.Common.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using LMS.Application.Features.Students.Queries.GetMyCourses;
using LMS.Application.Features.Students.Commands.CreateStudent;

namespace LMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public StudentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    //[Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<ApiResponse>> Create(CreateStudentCommand command)
    {
        var result = await _mediator.Send(command);
        return HandleResponse(this, result);
    }
    [HttpGet("my-courses")]
    public async Task<ActionResult<ApiResponse>> GetCourses([FromQuery]GetStudentCoursesQuery query)
    {
        var result = await _mediator.Send(query);
        return HandleResponse(this, result);
    }
}
