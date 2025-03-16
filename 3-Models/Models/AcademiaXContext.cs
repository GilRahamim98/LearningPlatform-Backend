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

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany() 
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany() 
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Progress>()
            .HasOne(p => p.User)
            .WithMany() 
            .OnDelete(DeleteBehavior.Cascade); 

        modelBuilder.Entity<Progress>()
            .HasOne(p => p.Lesson)
            .WithMany() 
            .OnDelete(DeleteBehavior.Cascade);

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

