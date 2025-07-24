using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models.DTOs.Dashboard;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace trainingCenter.Services.Foundation;

public class TenantDashboardService : ITenantDashboardService
{
    private readonly IStorageBroker storageBroker;

    public TenantDashboardService(IStorageBroker storageBroker)
    {
        this.storageBroker = storageBroker;
    }

    public async Task<TenantDashboardDto> GetTenantDashboardAsync(Guid tenantId)
    {
        var now = DateTime.UtcNow;
        var oneWeekAgo = now.AddDays(-7);

        var students = await storageBroker.SelectAll<Student>()
            .Where(s => s.TenantId == tenantId)
            .ToListAsync();

        var teachers = await storageBroker.SelectAll<User>()
            .Where(u => u.TenantId == tenantId && u.Role == Role.Teacher)
            .ToListAsync();

        var courses = await storageBroker.SelectAll<Course>()
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .ToListAsync();

        var payments = await storageBroker.SelectAll<Payment>()
            .Where(p => p.TenantId == tenantId)
            .ToListAsync();

        var notifications = await storageBroker.SelectAll<Notification>()
            .Where(n => n.TenantId == tenantId && n.SentAt.Date == now.Date)
            .ToListAsync();

        var studentCourses = await storageBroker.SelectAll<StudentCourse>()
            .Include(sc => sc.Student)
            .Include(sc => sc.Course)
            .Where(sc => sc.TenantId == tenantId)
            .ToListAsync();

        var last7DaysPayments = payments
            .Where(p => p.PaymentDate >= oneWeekAgo)
            .Sum(p => p.Amount);

        var categoryStats = payments
            .Where(p => p.Course.CategoryId.HasValue)
            .GroupBy(p => p.Course.CategoryId)
            .Select(g => new CategoryPaymentStatDto
            {
                CategoryId = g.Key.Value,
                TotalAmount = g.Sum(p => p.Amount)
            })
            .ToList();

        var latestStudents = students
            .OrderByDescending(s => s.EnrollmentDate)
            .Take(5)
            .ToList();

        var overdueStudents = studentCourses
            .Where(sc =>
                sc.IsActive &&
                sc.EnrollmentDate.AddMonths(1) < now)
            .Select(sc => new OverdueStudentDto
            {
                StudentId = sc.StudentId,
                FullName = sc.Student.FullName,
                CourseName = sc.Course.Name,
                EnrollmentDate = sc.EnrollmentDate,
                DaysOverdue = (now - sc.EnrollmentDate.AddMonths(1)).Days
            })
            .ToList();

        return new TenantDashboardDto
        {
            TotalStudents = students.Count,
            TotalTeachers = teachers.Count,
            TotalActiveCourses = courses.Count,
            TotalPayments = payments.Sum(p => p.Amount),
            MonthlyPayments = payments
                .Where(p => p.PaymentDate.Month == now.Month && p.PaymentDate.Year == now.Year)
                .Sum(p => p.Amount),
            TodayEvents = new List<string>(), 
            ImportantNotifications = notifications.Select(n => n.Message).ToList(),
            RecentStudents = latestStudents.Select(s => new StudentDto
            {
                Id = s.Id,
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber,
                EnrollmentDate = s.EnrollmentDate
            }).ToList(),
            MostActiveCourses = studentCourses
                .GroupBy(sc => sc.CourseId)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new MostActiveCourseDto
                {
                    CourseId = g.Key,
                    CourseName = g.First().Course.Name,
                    StudentCount = g.Count()
                }).ToList(),
            CategoryPaymentStats = categoryStats,
            Last7DaysPayments = last7DaysPayments,
            OverdueStudents = overdueStudents
        };
    }
}

