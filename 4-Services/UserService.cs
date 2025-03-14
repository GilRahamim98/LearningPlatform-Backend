﻿using System.Runtime.ConstrainedExecution;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Talent;

public class UserService : IDisposable
{
    private readonly AcademiaXContext _db;
    private readonly IMapper _mapper;

    public UserService(AcademiaXContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<string> Register(RegisterUserDto createUserDto)
    {
        User user = _mapper.Map<User>(createUserDto);
        user.Email = user.Email.ToLower();
        user.Password = PasswordHasher.HashPassword(user.Password);
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        user.Role = await _db.Roles.SingleAsync(r => r.RoleId == user.RoleId);
        return JwtHelper.GetNewToken(user);
    }

    public async Task<string?> Login(LoginUserDto loginDto)
    {
        loginDto.Email = loginDto.Email.ToLower();
        loginDto.Password = PasswordHasher.HashPassword(loginDto.Password);
        User? user = await _db.Users.AsNoTracking().Include(u=>u.Role).SingleOrDefaultAsync(u => u.Email == loginDto.Email && u.Password == loginDto.Password);
        if (user == null) return null;
        return JwtHelper.GetNewToken(user);
    }

    public bool IsStudentOrInstructor(int roleId)
    {
        return roleId == (int)RoleType.Student || roleId == (int)RoleType.Instructor;
    }

    public async Task<bool> UserExists(Guid id)
    {
        return await _db.Users.AsNoTracking().AnyAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExists(string email)
    {
        return await _db.Users.AsNoTracking().AnyAsync(u => u.Email == email);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
