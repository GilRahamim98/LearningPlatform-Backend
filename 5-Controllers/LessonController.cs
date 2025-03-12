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

    public LessonController(
        LessonService lessonService,
        ProgressService progressService,
        CourseService courseService,
        EnrollmentService enrollmentService,
        IValidator<CreateLessonDto> lessonValidator)
    {
        _lessonService = lessonService;
        _progressService = progressService;
        _courseService = courseService;
        _lessonValidator = lessonValidator;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllLessons()
    {
        List<LessonDto> lessons = await _lessonService.GetAllLessons();
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLessonById([FromRoute] Guid id)
    {
        LessonDto? lesson = await _lessonService.GetLessonById(id);
        if (lesson == null) return NotFound(new ResourceNotFound(id));
        return Ok(lesson);
    }

    [HttpGet("course/{courseId}")]
    public async Task<IActionResult> GetLessonsByCourse([FromRoute] Guid courseId)
    {
        List<LessonDto> lessons = await _lessonService.GetLessonsByCourseId(courseId);
        return Ok(lessons);
    }


    [HttpGet("course-preview/{courseId}")]
    public async Task<IActionResult> GetLessonsPreviewByCourse([FromRoute] Guid courseId)
    {
        List<LessonPreviewDto> lessons = await _lessonService.GetLessonsPreviewByCourse(courseId);
        return Ok(lessons);
    }


    [Authorize(Roles = "Admin,Instructor")]
    [HttpPost]
    public async Task<IActionResult> AddLesson([FromBody] CreateLessonDto createLessonDto)
    {
        ValidationResult validationResult = _lessonValidator.Validate(createLessonDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (!await _courseService.CourseExists(createLessonDto.CourseId)) errors.Add("Course not found");
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        LessonDto dbLesson = await _lessonService.AddLesson(createLessonDto);
        return Created("api/lessons/" + dbLesson.Id, dbLesson);
    }

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

    [Authorize(Roles = "Admin,Instructor")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson([FromRoute] Guid id, [FromBody] CreateLessonDto createLessonDto)
    {
        ValidationResult validationResult = _lessonValidator.Validate(createLessonDto);
        List<string> errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        if (!await _courseService.CourseExists(createLessonDto.CourseId)) errors.Add("Course not found");
        if (errors.Any())
        {
            return BadRequest(new ValidationError<List<string>>(errors));
        }
        LessonDto? dbLesson = await _lessonService.UpdateLesson(id, createLessonDto);
        if (dbLesson == null) return NotFound(new ResourceNotFound(id));
        return Ok(dbLesson);
    }

    [Authorize(Roles = "Admin,Instructor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson([FromRoute] Guid id)
    {
        bool deleted = await _lessonService.DeleteLesson(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

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

    [Authorize(Roles = "Admin,Instructor")]
    [HttpGet("progress-by-lesson/{lessonId}")]
    public async Task<IActionResult> GetProgressByLesson([FromRoute] Guid lessonId)
    {
        List<ProgressDto> progresses = await _progressService.GetProgressByLesson(lessonId);
        return Ok(progresses);
    }






    public void Dispose()
    {
        _lessonService.Dispose();
        _progressService.Dispose();
        _courseService.Dispose();
    }
}
