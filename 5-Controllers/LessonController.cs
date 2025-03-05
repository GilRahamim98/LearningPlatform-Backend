using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/Lessons")]
[ApiController]
public class LessonController : ControllerBase, IDisposable
{
    private readonly LessonService _lessonService;
    private readonly ProgressService _progressService;
    private readonly CourseService _courseService;

    private IValidator<CreateLessonDto> _lessonValidator;
    private IValidator<CreateProgressDto> _progressValidator;

    public LessonController(
        LessonService lessonService,
        ProgressService progressService,
        CourseService courseService,
        EnrollmentService enrollmentService,
        IValidator<CreateLessonDto> lessonValidator,
        IValidator<CreateProgressDto> progressValidator)
    {
        _lessonService = lessonService;
        _progressService = progressService;
        _courseService = courseService;
        _lessonValidator = lessonValidator;
        _progressValidator = progressValidator;
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson([FromRoute] Guid id)
    {
        bool deleted = await _lessonService.DeleteLesson(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

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
