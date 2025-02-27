using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class CourseService : IDisposable
{
    private readonly AcademiaXContext _db;

    public CourseService(AcademiaXContext db)
    {
        _db = db;
    }

    public List<Course> GetAllCourses()
    {
        return _db.Courses.AsNoTracking().ToList();
    }

    public Course? GetCourseById(Guid id)
    {
        return _db.Courses.AsNoTracking().SingleOrDefault(c => c.Id == id);
    }

    public Course AddCourse(Course course)
    {
        _db.Courses.Add(course);
        _db.SaveChanges();
        return course;
    }

    public Course? UpdateCourse(Course course)
    {
        Course? dbCourse = GetCourseById(course.Id);
        if (dbCourse == null) return null;
        _db.Courses.Attach(course);
        _db.Entry(course).State = EntityState.Modified;
        _db.SaveChanges();
        return course;
    }

    public bool DeleteCourse(Guid id)
    {
        Course? dbCourse = GetCourseById(id);
        if (dbCourse == null) return false;
        _db.Courses.Remove(dbCourse);
        _db.SaveChanges();
        return true;
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
