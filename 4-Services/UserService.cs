using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class UserService : IDisposable
{
    private readonly AcademiaXContext _db;

    public UserService(AcademiaXContext db)
    {
        _db = db;
    }

    public string Register(User user)
    {
        user.Email = user.Email.ToLower();
        user.Password = PasswordHasher.HashPassword(user.Password);
        _db.Users.Add(user);
        _db.SaveChanges();
        return JwtHelper.GetNewToken(user);

    }

    public bool EmailExists(string email)
    {
        return _db.Users.AsNoTracking().Any(u => u.Email == email);
    }

    public string? Login(Credentials credentials)
    {
        credentials.Email = credentials.Email.ToLower();
        credentials.Password = PasswordHasher.HashPassword(credentials.Password);
        User? user = _db.Users.AsNoTracking().SingleOrDefault(u => u.Email == credentials.Email && u.Password == credentials.Password);
        if (user == null) return null;
        return JwtHelper.GetNewToken(user);
    }


    public void Dispose()
    {
        _db.Dispose();
    }
}
