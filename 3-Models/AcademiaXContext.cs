using Microsoft.EntityFrameworkCore;

namespace Talent;

public class AcademiaXContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Progress> Progresses { get; set; }
    public DbSet<Role> Roles { get; set; }


    public AcademiaXContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(AppConfig.ConnectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        List<Role> roles = new List<Role>
        {
            new Role{RoleId = 1, RoleName= "Student" },
            new Role{RoleId = 2, RoleName= "Instructor" },
            new Role{RoleId = 3, RoleName= "Admin" }
        };

        modelBuilder.Entity<Role>().HasData(roles);
        base.OnModelCreating(modelBuilder);
    }
}

