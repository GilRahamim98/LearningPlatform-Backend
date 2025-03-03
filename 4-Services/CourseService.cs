using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Talent;

public class CourseService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;

    public CourseService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<CourseDTO>> GetAllCourses()
    {
        List<Course> courses = await _db.Courses.AsNoTracking().ToListAsync();
        return _mapper.Map<List<CourseDTO>>(courses);
    }

    public async Task<CourseDTO?> GetCourseById(Guid id)
    {
        Course? course = await _db.Courses.AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
        return _mapper.Map<CourseDTO?>(course);
    }

    public async Task<CourseDTO> AddCourse(CreateCourseDto createCourseDto)
    {
        Course course = _mapper.Map<Course>(createCourseDto);
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return _mapper.Map<CourseDTO>(course);
    }

    public async Task<CourseDTO?> UpdateCourse(Guid id, CreateCourseDto createCourseDto)
    {
        Course? dbCourse = await _db.Courses.FindAsync(id);
        if (dbCourse == null) return null;
        _mapper.Map(createCourseDto, dbCourse);
        await _db.SaveChangesAsync();
        return _mapper.Map<CourseDTO>(dbCourse);
    }

    public async Task<bool> DeleteCourse(Guid id)
    {
        Course? dbCourse = await _db.Courses.FindAsync(id);
        if (dbCourse == null) return false;
        _db.Courses.Remove(dbCourse);
        await _db.SaveChangesAsync();
        return true;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
