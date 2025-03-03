using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Talent;

[Route("api/courses")]
[ApiController]
public class CourseController : ControllerBase, IDisposable
{
    private readonly CourseService _courseService;
    public CourseController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        List<CourseDTO> courses = await _courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
    {
        CourseDTO? course = await _courseService.GetCourseById(id);
        if (course == null) return NotFound(new ResourceNotFound(id));
        return Ok(course);
    }

    [HttpPost]
    public async Task<IActionResult> AddCourse([FromBody] CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        CourseDTO dbCourse = await _courseService.AddCourse(createCourseDto);
        return Created("api/courses/" + dbCourse.Id, dbCourse);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] CreateCourseDto createCourseDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        CourseDTO? updatedCourse = await _courseService.UpdateCourse(id,createCourseDto);
        if (updatedCourse == null) return NotFound(new ResourceNotFound(id));
        return Ok(updatedCourse);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
    {
        bool deleted = await _courseService.DeleteCourse(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

    public void Dispose()
    {
        _courseService.Dispose();
    }
}
