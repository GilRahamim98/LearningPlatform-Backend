using Microsoft.EntityFrameworkCore;

namespace Talent;

public class ProgressService
{
    private readonly AcademiaXContext _db;

    public ProgressService(AcademiaXContext db)
    {
        _db = db;
    }

    public List<Progress> GetProgressByUser(Guid userId)
    {
        return _db.Progresses.AsNoTracking().Where(p => p.UserId == userId).Include(p => p.Lesson).ToList();
    }

    public List<Progress> GetProgressByLesson(Guid lessonId)
    {
        return _db.Progresses.AsNoTracking().Where(p => p.LessonId == lessonId).Include(p => p.User).ToList();
    }

    public Progress TrackLessonCompletion(Guid userId, Guid lessonId)
    {
        Progress? progress = _db.Progresses.AsNoTracking().SingleOrDefault(p => p.UserId == userId && p.LessonId == lessonId);

        if(progress == null)
        {
            progress = new Progress
            {
                UserId = userId,
                LessonId = lessonId,
                WatchedAt = DateTime.UtcNow
            };
            _db.Progresses.Add(progress);
        }
        else
        {
            progress.WatchedAt = DateTime.UtcNow;
            _db.Progresses.Update(progress);
        }
        _db.SaveChanges();
        return progress;

    }
}

