using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class LessonService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;

    public LessonService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    // Retrieves all lessons for a specific course and maps them to LessonDto
    public async Task<List<LessonDto>> GetLessonsByCourseId(Guid courseId)
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).ToListAsync();
        return _mapper.Map<List<LessonDto>>(lessons);
    }

    // Retrieves a preview of all lessons for a specific course and maps them to LessonPreviewDto
    public async Task<List<LessonPreviewDto>> GetLessonsPreviewByCourse(Guid courseId)
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).ToListAsync();
        return _mapper.Map<List<LessonPreviewDto>>(lessons);
    }

    // Adds a list of new lessons to the database and maps them to LessonDto
    public async Task<List<LessonDto>> AddLessons(List<CreateLessonDto> lessonsDto)
    {
        List<Lesson> lessons = _mapper.Map<List<Lesson>>(lessonsDto);
        await _db.Lessons.AddRangeAsync(lessons);
        await _db.SaveChangesAsync();
        return _mapper.Map<List<LessonDto>>(lessons);
    }

    // Updates a list of existing lessons in the database and maps them to LessonDto
    public async Task<List<LessonDto>> UpdateLessons(List<LessonDto> lessonsDto)
    {
        List<Lesson> updatedLessons = _mapper.Map<List<Lesson>>(lessonsDto);
        foreach(Lesson lesson in updatedLessons)
        {
            _db.Lessons.Attach(lesson);
            _db.Entry(lesson).State = EntityState.Modified;
        }
        await _db.SaveChangesAsync();
        return _mapper.Map<List<LessonDto>>(updatedLessons);
    }

    // Deletes a list of lessons from the database by their IDs
    public async Task<bool> DeleteLessons(List<Guid> ids)
    {
        List<Lesson> lessonsToDelete = await _db.Lessons.Where(lesson => ids.Contains(lesson.Id)).ToListAsync();
        if (lessonsToDelete.Count != ids.Count) return false;
        _db.Lessons.RemoveRange(lessonsToDelete);
        await _db.SaveChangesAsync();
        return true;
    }

    // Checks if a lesson exists in the database by its ID
    public async Task<bool> LessonExists(Guid id)
    {
        return await _db.Lessons.AnyAsync(l => l.Id == id);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
