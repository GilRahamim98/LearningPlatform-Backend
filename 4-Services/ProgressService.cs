using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class ProgressService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;


    public ProgressService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ProgressDto>> GetProgressByUser(Guid userId)
    {
        List<Progress> progresses = await _db.Progresses.AsNoTracking().Where(p => p.UserId == userId).Include(p => p.Lesson).ToListAsync();
        return _mapper.Map<List<ProgressDto>>(progresses);
    }

    public async Task<List<ProgressDto>> GetProgressByLesson(Guid lessonId)
    {
        List<Progress> progresses = await _db.Progresses.AsNoTracking().Where(p => p.LessonId == lessonId).Include(p => p.User).ToListAsync();
        return _mapper.Map<List<ProgressDto>>(progresses);

    }


    public async Task<ProgressDto> AddProgress(CreateProgressDto createProgressDto)
    {
        Progress progress = new Progress
        {
            UserId = createProgressDto.UserId,
            LessonId = createProgressDto.LessonId,
            WatchedAt = DateTime.Now
        };
        _db.Progresses.Add(progress);
        await _db.SaveChangesAsync();
        return _mapper.Map<ProgressDto>(progress);
    }

    public async Task<ProgressDto?> UpdateProgress(Guid id, CreateProgressDto createProgressDto)
    {
        Progress? dbProgress = await _db.Progresses.FindAsync(id);

        if (dbProgress == null) return null;

        _mapper.Map(createProgressDto, dbProgress);

        await _db.SaveChangesAsync();
        return _mapper.Map<ProgressDto>(dbProgress);

    }

    public void Dispose()
    {
        _db.Dispose();
    }
}

