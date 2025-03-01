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


    [HttpGet()]
    public IActionResult GetAllLessons()
    {
        List<Lesson> lessons = _lessonService.GetAllLessons();
        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public IActionResult GetLessonById([FromRoute] Guid id)
    {
        Lesson? lesson = _lessonService.GetLessonById(id);
        if (lesson == null) return NotFound(new ResourceNotFound(id));
        return Ok(lesson);
    }

    [HttpPost()]
    public IActionResult AddLesson([FromBody] Lesson lesson)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        Lesson dbLesson = _lessonService.AddLesson(lesson);
        return Created("api/lessons/" + dbLesson.Id, dbLesson);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateLesson([FromRoute] Guid id, [FromBody] Lesson lesson)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));
        lesson.Id = id;
        Lesson? dbLesson = _lessonService.UpdateLesson(lesson);
        if (dbLesson == null) return NotFound(new ResourceNotFound(id));
        return Ok(dbLesson);
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteLesson([FromRoute] Guid id)
    {
        bool deleted = _lessonService.DeleteLesson(id);
        if (!deleted) return NotFound(new ResourceNotFound(id));
        return NoContent();
    }

    public void Dispose()
    {
        _lessonService.Dispose();
    }
}
