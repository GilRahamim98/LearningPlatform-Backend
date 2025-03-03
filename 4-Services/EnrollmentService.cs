using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class EnrollmentService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;

    public EnrollmentService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<EnrollmentDto> EnrollUserInCourse(CreateEnrollmentDto createEnrollmentDto)
    {
        Enrollment enrollment = new Enrollment()
        {
            UserId = createEnrollmentDto.UserId,
            CourseId = createEnrollmentDto.CourseId,
            EnrolledAt = DateTime.Now
        };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();
        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<List<CourseDTO>> GetUserEnrollments(Guid userId)
    {
        List<Course> courses = await _db.Enrollments.AsNoTracking().Where(e => e.UserId == userId).Select(e => e.Course).ToListAsync();
        return _mapper.Map<List<CourseDTO>>(courses);
    }

    public async Task<List<UserDTO>> GetCourseEnrollments(Guid courseId)
    {
        List<User> users = await _db.Enrollments.AsNoTracking().Where(e => e.CourseId == courseId).Select(e => e.User).ToListAsync();
        return _mapper.Map<List<UserDTO>>(users);
    }

    public async Task<bool> UnenrollUserFromCourse(CreateEnrollmentDto createEnrollmentDto)
    {
        Enrollment? enrollment = _db.Enrollments.SingleOrDefault(e => e.UserId == createEnrollmentDto.UserId && e.CourseId == createEnrollmentDto.CourseId);

        if (enrollment == null) return false;
        _db.Enrollments.Remove(enrollment);
        await _db.SaveChangesAsync();
        return true;
    }


    public async Task<bool> IsUserEnrolled(Guid userId, Guid courseId)
    {
        return await _db.Enrollments.AsNoTracking().AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
