using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Talent;

[Route("api/courses")]
[ApiController]
public class CourseController : ControllerBase, IDisposable
{
    private readonly CourseService _courseService;
    private IValidator<CreateCourseDto> _courseValidator;

    public CourseController(
        CourseService courseService,
         IValidator<CreateCourseDto> courseValidator)
    {
        _courseService = courseService;
        _courseValidator = courseValidator;
    }

    // GET: api/courses
    // Retrieves all courses
    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        List<CourseDto> courses = await _courseService.GetAllCourses();
        return Ok(courses);
    }

    // GET: api/courses/{id}
    // Retrieves a course by its ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
    {
        CourseDto? course = await _courseService.GetCourseById(id);
        if (course == null) return NotFound(new ResourceNotFound(id));
        return Ok(course);
    }

    // POST: api/courses
    // Adds a new course (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
    [HttpPost]
    public async Task<IActionResult> AddCourse([FromBody] CreateCourseDto createCourseDto)
    {
        ValidationResult validationResult = _courseValidator.Validate(createCourseDto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        CourseDto dbCourse = await _courseService.AddCourse(createCourseDto);
        return Created("api/courses/" + dbCourse.Id, dbCourse);
    }

    // PUT: api/courses/{id}
    // Updates an existing course (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse([FromRoute] Guid id, [FromBody] CreateCourseDto createCourseDto)
    {
        ValidationResult validationResult = _courseValidator.Validate(createCourseDto);
        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        CourseDto? updatedCourse = await _courseService.UpdateCourse(id, createCourseDto);
        if (updatedCourse == null) return NotFound(new ResourceNotFound(id));
        return Ok(updatedCourse);
    }

    // DELETE: api/courses/{id}
    // Deletes a course (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
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
