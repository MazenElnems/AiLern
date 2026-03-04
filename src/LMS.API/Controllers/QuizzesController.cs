using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Quizzes.Commands.CreateQuiz;
using LMS.Application.Features.Quizzes.Queries.GetAllQuizzes;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizzesController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public QuizzesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Create(CreateQuizCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResponse(this, result);
        }

        [HttpGet("courses/{courseId}/quizzes")]
        public async Task<ActionResult<ApiResponse>> GetAllQuizzesByCourseId(int courseId,[FromQuery] GetAllQuizzesByCourseIdQuery query)
        {
            query.CourseId = courseId;
            var result = await _mediator.Send(query);
            return HandleResponse(this, result);
        }
        
    }
}
