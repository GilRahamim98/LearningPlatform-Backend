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

    public async Task<List<LessonDto>> GetAllLessons()
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().ToListAsync();
        return _mapper.Map<List<LessonDto>>(lessons);
    }

    public async Task<LessonDto?> GetLessonById(Guid id)
    {
        Lesson? lesson = await _db.Lessons.AsNoTracking().SingleOrDefaultAsync(l => l.Id == id);
        return _mapper.Map<LessonDto?>(lesson);
    }

    public async Task<List<LessonDto>> GetLessonsByCourseId(Guid courseId)
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).ToListAsync();
        return _mapper.Map<List<LessonDto>>(lessons);
    }

    public async Task<List<LessonPreviewDto>> GetLessonsPreviewByCourse(Guid courseId)
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().Where(l => l.CourseId == courseId).ToListAsync();
        return _mapper.Map<List<LessonPreviewDto>>(lessons);
    }

    public async Task<LessonDto> AddLesson(CreateLessonDto createLessonDto)
    {
        Lesson lesson = _mapper.Map<Lesson>(createLessonDto);
        await _db.Lessons.AddAsync(lesson);
        await _db.SaveChangesAsync();
        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<List<LessonDto>> AddLessons(List<CreateLessonDto> lessonsDto)
    {
        List<Lesson> lessons = _mapper.Map<List<Lesson>>(lessonsDto);
        await _db.Lessons.AddRangeAsync(lessons);
        await _db.SaveChangesAsync();
        return _mapper.Map<List<LessonDto>>(lessons);
    }

    public async Task<LessonDto?> UpdateLesson(Guid id, CreateLessonDto createLessonDto)
    {
        Lesson? dbLesson = await _db.Lessons.FindAsync(id);
        if (dbLesson == null) return null;
        _mapper.Map(createLessonDto, dbLesson);
        await _db.SaveChangesAsync();
        return _mapper.Map<LessonDto>(dbLesson);
    }

    public async Task<bool> DeleteLesson(Guid id)
    {
        Lesson? dbLesson = await _db.Lessons.FindAsync(id);
        if (dbLesson == null) return false;
        _db.Lessons.Remove(dbLesson);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLessons(List<Guid> ids)
    {
        List<Lesson> lessonsToDelete = await _db.Lessons.Where(lesson => ids.Contains(lesson.Id)).ToListAsync();
        if (lessonsToDelete.Count != ids.Count) return false;
        _db.Lessons.RemoveRange(lessonsToDelete);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LessonExists(Guid id)
    {
        return await _db.Lessons.AnyAsync(l=> l.Id == id);
    }



    public void Dispose()
    {
        _db.Dispose();
    }
}
