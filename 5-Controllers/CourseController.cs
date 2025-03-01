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

    [HttpGet()]
    public IActionResult GetAllCourses()
    {
        List<Course> courses = _courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public IActionResult GetCourseById([FromRoute] Guid id)
    {
        Course? course = _courseService.GetCourseById(id);
        if (course == null) return NotFound(new ResourceNotFound(id));
        return Ok(course);
    }

    [HttpPost()]
    public IActionResult AddCourse([FromBody] Course course)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        Course dbCourse = _courseService.AddCourse(course);
        return Created("api/courses/" + dbCourse.Id, dbCourse);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateCourse([FromRoute] Guid id, [FromBody] Course course)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        course.Id = id;
        Course? dbCourse = _courseService.UpdateCourse(course);
        if (dbCourse == null) return NotFound(new ResourceNotFound(id));
        return Ok(dbCourse);
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteCourse([FromRoute] Guid id)
    {
        bool deleted = _courseService.DeleteCourse(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

    public void Dispose()
    {
        _courseService.Dispose();
    }
}
