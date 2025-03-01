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
    public IActionResult EnrollUser([FromBody] EnrollmentCredentials enrollmentCredentials)
    {
        bool alreadyEnrolled = _enrollmentService.IsUserEnrolled(enrollmentCredentials.UserId, enrollmentCredentials.CourseId);
        if (alreadyEnrolled) return BadRequest(new ValidationError("User is already enrolled in this course."));

        Enrollment newEnrollment = _enrollmentService.EnrollUserInCourse(enrollmentCredentials.UserId, enrollmentCredentials.CourseId);
        return Created("/api/enrollments", newEnrollment);
    }

    [HttpGet("enrollments-by-user/{userId}")]
    public IActionResult GetUserEnrollments([FromRoute] Guid userId)
    {
        List<Course> courses = _enrollmentService.GetUserEnrollments(userId);
        return Ok(courses);
    }

    [HttpGet("enrollments-by-course/{courseId}")]
    public IActionResult GetCourseEnrollments([FromRoute] Guid courseId)
    {
        List<User> users = _enrollmentService.GetCourseEnrollments(courseId);
        return Ok(users);
    }

    [HttpDelete]
    public IActionResult UnenrollUser([FromBody] EnrollmentCredentials enrollmentCredentials)
    {
        bool deleted = _enrollmentService.UnenrollUserFromCourse(enrollmentCredentials.UserId,enrollmentCredentials.CourseId);
        if (!deleted) return NotFound(new ResourceNotFound($"Enrollment not found for this user id : {enrollmentCredentials.UserId} and for this course id : {enrollmentCredentials.CourseId}"));
        return NoContent();
    }

    public void Dispose()
    {
        _enrollmentService.Dispose();
    }
}
