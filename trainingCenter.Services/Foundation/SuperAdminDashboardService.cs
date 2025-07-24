using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models.DTOs.Dashboard;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;
using Microsoft.EntityFrameworkCore;

public class SuperAdminDashboardService : ISuperAdminDashboardService
{
    private readonly IStorageBroker storageBroker;

    public SuperAdminDashboardService(IStorageBroker storageBroker)
    {
        this.storageBroker = storageBroker;
    }

    public async Task<SuperAdminDashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var last7Days = now.AddDays(-7);

        var tenants = await storageBroker.SelectAll<Tenant>().ToListAsync();
        var students = await storageBroker.SelectAll<Student>().ToListAsync();
        var teachers = await storageBroker.SelectAll<User>()
            .Where(u => u.Role == Role.Teacher)
            .ToListAsync();
        var courses = await storageBroker.SelectAll<Course>()
            .Where(c => c.IsActive)
            .ToListAsync();
        var payments = await storageBroker.SelectAll<Payment>().ToListAsync();

        var topTenants = tenants.Select(t =>
        {
            var tenantStudents = students.Where(s => s.TenantId == t.Id).Count();
            var tenantPayments = payments.Where(p => p.TenantId == t.Id).Sum(p => p.Amount);

            return new TenantActivityDto
            {
                TenantId = t.Id,
                TenantName = t.Name,
                StudentCount = tenantStudents,
                TotalPayments = tenantPayments
            };
        })
        .OrderByDescending(t => t.TotalPayments)
        .Take(5)
        .ToList();

        return new SuperAdminDashboardDto
        {
            TotalTenants = tenants.Count,
            TotalStudents = students.Count,
            TotalTeachers = teachers.Count,
            TotalActiveCourses = courses.Count,
            TotalPayments = payments.Sum(p => p.Amount),
            Last7DaysPayments = payments
                .Where(p => p.PaymentDate >= last7Days)
                .Sum(p => p.Amount),
            Last7DaysNewStudents = students
                .Where(s => s.EnrollmentDate >= last7Days)
                .Count(),
            TopTenants = topTenants
        };
    }
}
