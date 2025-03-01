using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class EnrollmentService : IDisposable
{
    private readonly AcademiaXContext _db;

    public EnrollmentService(AcademiaXContext db)
    {
        _db = db;
    }

    public Enrollment EnrollUserInCourse(Guid userId, Guid courseId)
    {
        Enrollment enrollment = new Enrollment()
        {
            UserId = userId,
            CourseId = courseId,
            EnrolledAt = DateTime.UtcNow
        };
        _db.Enrollments.Add(enrollment);
        _db.SaveChanges();
        return enrollment;
    }

    public List<Course> GetUserEnrollments(Guid userId)
    {
        return _db.Enrollments.AsNoTracking().Where(e => e.UserId == userId).Select(e => e.Course).ToList();
    }

    public List<User> GetCourseEnrollments(Guid courseId)
    {
        return _db.Enrollments.AsNoTracking().Where(e => e.CourseId == courseId).Select(e => e.User).ToList();
    }

    public bool UnenrollUserFromCourse(Guid userId, Guid courseId)
    {
        Enrollment? enrollment = _db.Enrollments.SingleOrDefault(e => e.UserId == userId && e.CourseId == courseId);

        if (enrollment == null) return false;
        _db.Enrollments.Remove(enrollment);
        _db.SaveChanges();
        return true;
    }


    public bool IsUserEnrolled(Guid userId, Guid courseId)
    {
        return _db.Enrollments.AsNoTracking().Any(e => e.UserId == userId && e.CourseId == courseId);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
