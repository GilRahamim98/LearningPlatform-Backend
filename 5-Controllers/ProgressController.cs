using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Talent;

[Route("api/progresses")]
[ApiController]
public class ProgressController : ControllerBase,IDisposable
{
    private readonly ProgressService _progressService;
    public ProgressController(ProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("progress-by-user/{userId}")]
    public async Task<IActionResult> GetProgressByUser([FromRoute] Guid userId)
    {
        List<ProgressDto> progresses = await _progressService.GetProgressByUser(userId);
        return Ok(progresses);
    }

    [HttpGet("progress-by-lesson/{lessonId}")]
    public async Task<IActionResult> GetProgressByLesson([FromRoute] Guid lessonId)
    {
        List<ProgressDto> progresses = await _progressService.GetProgressByLesson(lessonId);
        return Ok(progresses);
    }

    [HttpPost]
    public async Task<IActionResult> AddProgress([FromBody] CreateProgressDto createProgressDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        ProgressDto dbProgress = await _progressService.AddProgress(createProgressDto);
        return Created("api/progresses/" + dbProgress.Id, dbProgress);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProgress([FromRoute] Guid id,[FromBody] CreateProgressDto createProgressDto)
    {
        if (!ModelState.IsValid) return BadRequest(new ValidationError(ModelState.GetAllErrors()));

        ProgressDto? dbProgress = await _progressService.UpdateProgress(id,createProgressDto);
        return Ok(dbProgress);
    }



    public void Dispose()
    {
        _progressService.Dispose();
    }
}
