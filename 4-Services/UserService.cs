using Microsoft.EntityFrameworkCore;

namespace Talent;

public class UserService :IDisposable
{
    private readonly AcademiaXContext _db;

    public UserService(AcademiaXContext db)
    {
        _db = db;
    }

    public void Register(User user)
    {
        user.Email = user.Email.ToLower();
        _db.Users.Add(user);
        _db.SaveChanges();

    }

    public bool EmailExists(string email)
    {
        return _db.Users.AsNoTracking().Any(u=> u.Email == email);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
