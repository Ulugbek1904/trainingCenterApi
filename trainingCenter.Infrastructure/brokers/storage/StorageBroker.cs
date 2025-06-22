using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;

namespace trainingCenter.Infrastructure.brokers.storage;

public class StorageBroker : DbContext, IStorageBroker
{
    private readonly IConfiguration configuration;

    public StorageBroker(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<StudentCourse> StudentCourses => Set<StudentCourse>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Notification> Notification => Set<Notification>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Category> Categories => Set<Category>();

    public IQueryable<T> SelectAll<T>() where T : class
        => Set<T>().AsQueryable();

    public async ValueTask<T?> SelectByKeyAsync<T>(params object[] keyValues) where T : class
        => await this.Set<T>().FindAsync(keyValues);

    public async ValueTask<T> SelectByIdAsync<T>(Guid id) where T : class
        => await this.Set<T>().FindAsync(id) ?? throw new InvalidOperationException($"Entity with ID {id} not found.");

    public async ValueTask<T> InsertAsync<T>(T entity) where T : class
    {
        await this.Set<T>().AddAsync(entity);
        await this.SaveChangesAsync();
        return entity;
    }

    public async ValueTask<T> UpdateAsync<T>(T entity) where T : class
    {
        this.Set<T>().Update(entity);
        await this.SaveChangesAsync();
        return entity;
    }

    public async ValueTask<T> DeleteAsync<T>(T entity) where T : class
    {
        this.Set<T>().Remove(entity);
        await this.SaveChangesAsync();
        return entity;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = this.configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseNpgsql(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Primary Keys
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Student>().HasKey(s => s.Id);
        modelBuilder.Entity<Course>().HasKey(c => c.Id);
        modelBuilder.Entity<StudentCourse>().HasKey(sc => new { sc.StudentId, sc.CourseId });
        modelBuilder.Entity<Attendance>().HasKey(a => a.Id);
        modelBuilder.Entity<Grade>().HasKey(g => g.Id);
        modelBuilder.Entity<Payment>().HasKey(p => p.Id);
        modelBuilder.Entity<Notification>().HasKey(n => n.Id);
        modelBuilder.Entity<Category>().HasKey(c => c.Id);
        modelBuilder.Entity<RefreshToken>().HasKey(rt => rt.Id);

        // Relationships
        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Student)
            .WithMany(s => s.StudentCourses)
            .HasForeignKey(sc => sc.StudentId);

        modelBuilder.Entity<StudentCourse>()
            .HasOne(sc => sc.Course)
            .WithMany(c => c.StudentCourses)
            .HasForeignKey(sc => sc.CourseId);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany()
            .HasForeignKey(c => c.TeacherId);

        modelBuilder.Entity<Course>()
            .HasOne(c => c.Category)
            .WithMany(c => c.Courses)
            .HasForeignKey(c => c.CategoryId);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student)
            .WithMany()
            .HasForeignKey(a => a.StudentId);

        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Course)
            .WithMany()
            .HasForeignKey(a => a.CourseId);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Student)
            .WithMany()
            .HasForeignKey(g => g.StudentId);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Course)
            .WithMany()
            .HasForeignKey(g => g.CourseId);

        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Teacher)
            .WithMany()
            .HasForeignKey(g => g.TeacherId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Student)
            .WithMany()
            .HasForeignKey(p => p.StudentId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Course)
            .WithMany()
            .HasForeignKey(p => p.CourseId);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.Student)
            .WithMany()
            .HasForeignKey(n => n.StudentId)
            .IsRequired(false);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId);

        // JSONB Fields
        modelBuilder.Entity<Course>()
            .Property(c => c.Materials)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Student>()
            .Property(s => s.Notes)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Attendance>()
            .Property(a => a.Notes)
            .HasColumnType("jsonb");

        modelBuilder.Entity<StudentCourse>()
            .Property(sc => sc.Notes)
            .HasColumnType("jsonb");

        modelBuilder.Entity<Payment>()
            .Property(p => p.InstallmentPlan)
            .HasColumnType("jsonb");

        // Enum Types
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();

        modelBuilder.Entity<Attendance>()
            .Property(a => a.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Channel)
            .HasConversion<string>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Priority)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentMethod)
            .HasConversion<string>();

        modelBuilder.Entity<Course>()
            .Property(c => c.Level)
            .HasConversion<string>();

        // Default Values
        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Student>()
            .Property(s => s.EnrollmentDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Attendance>()
            .Property(a => a.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Grade>()
            .Property(g => g.Date)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Payment>()
            .Property(p => p.PaymentDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Notification>()
            .Property(n => n.SentAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<StudentCourse>()
            .Property(sc => sc.EnrollmentDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}
