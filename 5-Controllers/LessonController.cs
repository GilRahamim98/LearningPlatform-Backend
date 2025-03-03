using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/Lessons")]
[ApiController]
public class LessonController : ControllerBase, IDisposable
{
    private readonly LessonService _lessonService;
    public LessonController(LessonService lessonService)
    {
        _lessonService = lessonService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllLessons()
    {
        List<LessonDTO> lessons = await _lessonService.GetAllLessons();
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLessonById([FromRoute] Guid id)
    {
        LessonDTO? lesson = await _lessonService.GetLessonById(id);
        if (lesson == null) return NotFound(new ResourceNotFound(id));
        return Ok(lesson);
    }

    [HttpPost]
    public async Task<IActionResult> AddLesson([FromBody] CreateLessonDto createLessonDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        LessonDTO dbLesson = await _lessonService.AddLesson(createLessonDto);
        return Created("api/lessons/" + dbLesson.Id, dbLesson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson([FromRoute] Guid id, [FromBody] CreateLessonDto createLessonDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        LessonDTO? dbLesson = await _lessonService.UpdateLesson(id, createLessonDto);
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

    public void Dispose()
    {
        _lessonService.Dispose();
    }
}
