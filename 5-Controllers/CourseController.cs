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
    private readonly EnrollmentService _enrollmentService;

    private IValidator<CreateCourseDto> _courseValidator;

    public CourseController(
        CourseService courseService,
        EnrollmentService enrollmentService,
         IValidator<CreateEnrollmentDto> enrollmentValidator,
         IValidator<CreateCourseDto> courseValidator)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _courseValidator = courseValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCourses()
    {
        List<CourseDto> courses = await _courseService.GetAllCourses();
        return Ok(courses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourseById([FromRoute] Guid id)
    {
        CourseDto? course = await _courseService.GetCourseById(id);
        if (course == null) return NotFound(new ResourceNotFound(id));
        return Ok(course);
    }
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

    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse([FromRoute] Guid id)
    {
        bool deleted = await _courseService.DeleteCourse(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpGet("enrollments-by-course/{courseId}")]
    public async Task<IActionResult> GetCourseEnrollments([FromRoute] Guid courseId)
    {
        List<UserDto> users = await _enrollmentService.GetCourseEnrollments(courseId);
        return Ok(users);
    }

    public void Dispose()
    {
        _courseService.Dispose();
    }
}
