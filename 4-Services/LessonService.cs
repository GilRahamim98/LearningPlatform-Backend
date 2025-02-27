using Microsoft.EntityFrameworkCore;

namespace Talent;

public class LessonService : IDisposable
{
    private readonly AcademiaXContext _db;

    public LessonService(AcademiaXContext db)
    {
        _db = db;
    }

    public List<Lesson> GetAllLessons()
    {
        return _db.Lessons.AsNoTracking().ToList();
    }

    public Lesson? GetLessonById(Guid id)
    {
        return _db.Lessons.AsNoTracking().SingleOrDefault(l => l.Id == id);
    }

    public Lesson AddLesson(Lesson lesson)
    {
        _db.Lessons.Add(lesson);
        _db.SaveChanges();
        return lesson;
    }

    public Lesson? UpdateLesson(Lesson lesson)
    {
        Lesson? dbLesson = GetLessonById(lesson.Id);
        if (dbLesson == null) return null;
        _db.Lessons.Attach(lesson);
        _db.Entry(lesson).State = EntityState.Modified;
        _db.SaveChanges();
        return lesson;
    }

    public bool DeleteLesson(Guid id)
    {
        Lesson? dbLesson = GetLessonById(id);
        if (dbLesson == null) return false;
        _db.Lessons.Remove(dbLesson);
        _db.SaveChanges();
        return true;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
