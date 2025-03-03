using System.Runtime.ConstrainedExecution;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public enum RoleType
{
    Member = 1,
    Instructor = 2,
    Admin = 3
}

public class UserService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;


    public UserService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;

    }

    public async Task<string> Register(CreateUserDto createUserDto)
    {
        User user = _mapper.Map<User>(createUserDto);
        user.Email = user.Email.ToLower();
        user.Password = PasswordHasher.HashPassword(user.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return JwtHelper.GetNewToken(user);

    }

    public async Task<bool> EmailExists(string email)
    {
        return await _db.Users.AsNoTracking().AnyAsync(u => u.Email == email.ToLower());
    }

    public async Task<string?> Login(LoginDto loginDto)
    {
        loginDto.Email = loginDto.Email.ToLower();
        loginDto.Password = PasswordHasher.HashPassword(loginDto.Password);
        User? user = await _db.Users.AsNoTracking().SingleOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);
        if (user == null) return null;
        return JwtHelper.GetNewToken(user);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
