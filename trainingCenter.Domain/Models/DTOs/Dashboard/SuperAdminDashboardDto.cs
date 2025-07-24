namespace trainingCenter.Domain.Models.DTOs.Dashboard;

public class SuperAdminDashboardDto
{
    public int TotalTenants { get; set; }
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalActiveCourses { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal Last7DaysPayments { get; set; }
    public int Last7DaysNewStudents { get; set; }
    public List<TenantActivityDto> TopTenants { get; set; } = new();
}

public class TenantActivityDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; }
    public int StudentCount { get; set; }
    public decimal TotalPayments { get; set; }
}
