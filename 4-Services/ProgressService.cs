using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class ProgressService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly EnrollmentService _enrollmentService;
    private readonly IMapper _mapper;


    public ProgressService(AcademiaXContext db,EnrollmentService enrollmentService, IMapper mapper)
    {
        _db = db;
        _enrollmentService = enrollmentService;
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

    public async Task<bool> IsUserEnrolled(CreateProgressDto createProgressDto)
    {
        Lesson? lesson = await _db.Lessons.FindAsync(createProgressDto.LessonId);
        if (lesson == null) return false;
        return await _enrollmentService.IsUserEnrolled(createProgressDto.UserId, lesson.CourseId);  
    } 

    public void Dispose()
    {
        _db.Dispose();
        _enrollmentService.Dispose();
    }
}

