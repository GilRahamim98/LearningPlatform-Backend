using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/lessons")]
[ApiController]
public class LessonController : ControllerBase, IDisposable
{
    private readonly LessonService _lessonService;
    private readonly ProgressService _progressService;
    private readonly CourseService _courseService;

    private IValidator<CreateLessonDto> _lessonValidator;
    private IValidator<LessonDto> _lessonDtoValidator;

    public LessonController(
        LessonService lessonService,
        ProgressService progressService,
        CourseService courseService,
        EnrollmentService enrollmentService,
        IValidator<CreateLessonDto> lessonValidator,
        IValidator<LessonDto> lessonDtoValidator)
    {
        _lessonService = lessonService;
        _progressService = progressService;
        _courseService = courseService;
        _lessonValidator = lessonValidator;
        _lessonDtoValidator = lessonDtoValidator;
    }

    // GET: api/lessons/course/{courseId}
    // Retrieves all lessons for a specific course (only accessible by authorized users)
    [Authorize]
    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetLessonsByCourse([FromRoute] Guid courseId)
    {
        List<LessonDto> lessons = await _lessonService.GetLessonsByCourseId(courseId);
        return Ok(lessons);
    }

    // GET: api/lessons/course-preview/{courseId}
    // Retrieves a preview of all lessons for a specific course
    [HttpGet("course-preview/{courseId}")]
    public async Task<IActionResult> GetLessonsPreviewByCourse([FromRoute] Guid courseId)
    {
        List<LessonPreviewDto> lessons = await _lessonService.GetLessonsPreviewByCourse(courseId);
        return Ok(lessons);
    }

    // POST: api/lessons/add-multiple
    // Adds multiple new lessons (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
    [HttpPost("add-multiple")]
    public async Task<IActionResult> AddLessons([FromBody] List<CreateLessonDto> lessonsDto)
    {
        List<string> errors = [];
        foreach (CreateLessonDto lesson in lessonsDto)
        {
            ValidationResult validationResult = _lessonValidator.Validate(lesson);
            errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            if (!await _courseService.CourseExists(lesson.CourseId)) errors.Add("Course not found");
        }
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        List<LessonDto> dbLessons = await _lessonService.AddLessons(lessonsDto);
        return Created("api/lessons/list" , dbLessons);
    }

    // PUT: api/lessons/update-multiple
    // Updates multiple existing lessons (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
    [HttpPut("update-multiple")]
    public async Task<IActionResult> UpdateLessons([FromBody] List<LessonDto> lessons)
    {
        if (lessons == null || !lessons.Any())
            return BadRequest("No lessons provided");
        List<string> errors = [];
        foreach (LessonDto lesson in lessons)
        {
            ValidationResult validationResult = _lessonDtoValidator.Validate(lesson);
            errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            if (!await _courseService.CourseExists(lesson.CourseId)) errors.Add("Course not found");
        }
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        List<LessonDto> dbLessons = await _lessonService.UpdateLessons(lessons);
        if (!dbLessons.Any()) return NotFound(new ResourceNotFound("None of the specified lessons were found"));
        return Ok(dbLessons);
    }

    // DELETE: api/lessons/delete-multiple
    // Deletes multiple lessons (only accessible by Admin and Instructor roles)
    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("delete-multiple")]
    public async Task<IActionResult> DeleteLessons([FromBody] List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return BadRequest("No lesson ids provided");
        bool deleted = await _lessonService.DeleteLessons(ids);
        if (!deleted) return NotFound(new ResourceNotFound("Some lessons could not be found"));
        return NoContent();
    }



    public void Dispose()
    {
        _lessonService.Dispose();
        _progressService.Dispose();
        _courseService.Dispose();
    }
}
