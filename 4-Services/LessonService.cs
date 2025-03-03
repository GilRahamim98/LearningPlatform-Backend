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

    public async Task<List<LessonDTO>> GetAllLessons()
    {
        List<Lesson> lessons = await _db.Lessons.AsNoTracking().ToListAsync();
        return _mapper.Map<List<LessonDTO>>(lessons);
    }

    public async Task<LessonDTO?> GetLessonById(Guid id)
    {
        Lesson? lesson = await _db.Lessons.AsNoTracking().SingleOrDefaultAsync(l => l.Id == id);
        return _mapper.Map<LessonDTO?>(lesson);
    }

    public async Task<LessonDTO> AddLesson(CreateLessonDto createLessonDto)
    {
        Lesson lesson = _mapper.Map<Lesson>(createLessonDto);
        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync();
        return _mapper.Map<LessonDTO>(lesson);
    }

    public async Task<LessonDTO?> UpdateLesson(Guid id, CreateLessonDto createLessonDto)
    {
        Lesson? dbLesson = await _db.Lessons.FindAsync(id);
        if (dbLesson == null) return null;
        _mapper.Map(createLessonDto, dbLesson);
        await _db.SaveChangesAsync();
        return _mapper.Map<LessonDTO>(dbLesson);
    }

    public async Task<bool> DeleteLesson(Guid id)
    {
        Lesson? dbLesson = await _db.Lessons.FindAsync(id);
        if (dbLesson == null) return false;
        _db.Lessons.Remove(dbLesson);
        await _db.SaveChangesAsync();
        return true;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
