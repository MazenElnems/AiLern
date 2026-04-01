using LMS.API.Controllers.Common;
using LMS.API.Models;
using LMS.Application.Features.Dashboards.Queries.GetQuizDashboard;
using LMS.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = UserRoles.Instructor)]
        [HttpGet("quiz/{quizId}")]
        public async Task<ActionResult<ApiResponse>> GetQuizDashboard([FromRoute]GetQuizDashboardQuery query)
        {
            var result = await _mediator.Send(query);
            return HandleResponse(this, result);
        }
    }
}
