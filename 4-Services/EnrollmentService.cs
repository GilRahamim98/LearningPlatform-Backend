using Serilog;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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

    // Enrolls a user in a course and returns the enrollment details
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

    // Retrieves all enrollments for a specific user
    public async Task<List<EnrollmentDto>> GetUserEnrollments(Guid userId)
    {
        List<Enrollment> enrollments = await _db.Enrollments.AsNoTracking().Where(e => e.UserId == userId).ToListAsync();
        return _mapper.Map<List<EnrollmentDto>>(enrollments);
    }

    // Unenrolls a user from a course and removes related progress records
    public async Task<bool> UnenrollUserFromCourse(Guid enrollmentId)
    {
        await using IDbContextTransaction transaction = _db.Database.BeginTransaction();

        try
        {
            Enrollment? enrollment = _db.Enrollments.SingleOrDefault(e => e.Id == enrollmentId);
            if (enrollment == null) return false;

            List<Progress> progresses = await _db.Progresses.Where(p=>p.UserId == enrollment.UserId && p.Lesson.CourseId == enrollment.CourseId).ToListAsync();
            _db.Progresses.RemoveRange(progresses);
            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex.Message);
            return false;

        }
    }
    
    // Checks if a user is enrolled in a specific course
    public async Task<bool> IsUserEnrolled(Guid userId, Guid courseId)
    {
        return await _db.Enrollments.AsNoTracking().AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
