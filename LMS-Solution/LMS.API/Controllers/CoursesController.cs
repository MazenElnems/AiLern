using LMS.Core.Services.Courses.Interfaces;
using LMS.Shared.DTOs.Courses;
using LMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace LMS.API.Controllers;

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
        int instructorId = 6;
        int id = await _courseService.CreateAsync(dto, instructorId);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetAllCoursesDto>>> GetAll([FromQuery] CouseQueryDto query)
    {
        var dto = await _courseService.GetAllCoursesAsync(query);
        return dto;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetCourseDto>> GetById(int id)
    {
        var dto = await _courseService.GetByIdAsync(id);
    
        if (dto is null)
            return NotFound($"Invalid course id {id}");

        return dto;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool isDeleted = await _courseService.DeleteAsync(id);

        if (!isDeleted)
            return NotFound();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateCourseDto dto)
    {
        dto.Id = id;
        bool isUpdated  = await _courseService.UpdateAsync(dto);
        if (!isUpdated)
            return NotFound();

        return Ok(dto);
    }
}