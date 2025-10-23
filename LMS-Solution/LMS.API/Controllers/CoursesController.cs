using LMS.Core.DTOs.Course;
using LMS.Core.Models;
using LMS.Core.Services.Courses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Instructor)]
        public async Task<IActionResult> Create(CreateCourseDto dto)
        {
            int instructorId = Convert.ToInt32(User.FindFirst("uid")?.Value);
            int id = await _courseService.CreateAsync(dto, instructorId);
            return Ok(id);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetAllCoursesDto>>> GetAll()
        {
            var dtos = await _courseService.GetAllCoursesAsync();
            return dtos;
        }
        
    }
}
