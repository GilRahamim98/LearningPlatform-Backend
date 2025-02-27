using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Talent;

public class CourseController : ControllerBase, IDisposable
{
    private readonly CourseService _courseService;
    public CourseController(CourseService courseService)
    {
        _courseService = courseService;
    }

    [HttpGet("api/courses")]
    public IActionResult GetAllCourses()
    {
        List<Course> courses = _courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("api/courses/{id}")]
    public IActionResult GetCourseById([FromRoute] Guid id)
    {
        Course? course = _courseService.GetCourseById(id);
        if (course == null) return NotFound("Course with this id not found.");
        return Ok(course);
    }

    [HttpPost("api/courses")]
    public IActionResult AddCourse([FromBody] Course course)
    {
        if (!ModelState.IsValid) return BadRequest("Course not valid.");

        Course dbCourse = _courseService.AddCourse(course);
        return Created("api/courses/" + dbCourse.Id, dbCourse);
    }

    [HttpPut("api/courses/{id}")]
    public IActionResult UpdateCourse([FromRoute] Guid id, [FromBody] Course course)
    {
        if (!ModelState.IsValid) return BadRequest("Course not valid.");
        course.Id= id;
        Course? dbCourse = _courseService.UpdateCourse(course);
        if (dbCourse == null) return NotFound("Course with this id not found.");
        return Ok(dbCourse);
    }


    [HttpDelete("api/courses/{id}")]
    public IActionResult DeleteProduct([FromRoute] Guid id)
    {
        bool deleted = _courseService.DeleteCourse(id);
        if (!deleted) return NotFound("Course with this id not found.");
        return NoContent();
    }
    public void Dispose()
    {
        _courseService.Dispose();
    }
}
