using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/enrollments")]
[ApiController]
public class EnrollmentController : ControllerBase, IDisposable
{

    private readonly EnrollmentService _enrollmentService;

    public EnrollmentController(EnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost]
    public async Task<IActionResult> EnrollUser([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        bool alreadyEnrolled = await _enrollmentService.IsUserEnrolled(createEnrollmentDto.UserId, createEnrollmentDto.CourseId);
        if (alreadyEnrolled) return BadRequest(new ValidationError("User is already enrolled in this course."));
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        EnrollmentDto newEnrollment = await _enrollmentService.EnrollUserInCourse(createEnrollmentDto);
        return Created("/api/enrollments", newEnrollment);
    }

    [HttpGet("enrollments-by-user/{userId}")]
    public async Task<IActionResult> GetUserEnrollments([FromRoute] Guid userId)
    {
        List<CourseDTO> courses = await _enrollmentService.GetUserEnrollments(userId);
        return Ok(courses);
    }

    [HttpGet("enrollments-by-course/{courseId}")]
    public async Task<IActionResult> GetCourseEnrollments([FromRoute] Guid courseId)
    {
        List<UserDTO> users = await _enrollmentService.GetCourseEnrollments(courseId);
        return Ok(users);
    }

    [HttpDelete]
    public async Task<IActionResult> UnenrollUser([FromBody] CreateEnrollmentDto createEnrollmentDto)
    {
        bool deleted = await _enrollmentService.UnenrollUserFromCourse(createEnrollmentDto);
        if (!deleted) return NotFound(new ResourceNotFound($"Enrollment not found for this user id : {createEnrollmentDto.UserId} and for this course id : {createEnrollmentDto.CourseId}"));
        return NoContent();
    }

    public void Dispose()
    {
        _enrollmentService.Dispose();
    }
}
