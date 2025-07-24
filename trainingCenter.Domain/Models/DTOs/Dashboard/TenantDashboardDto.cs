namespace trainingCenter.Domain.Models.DTOs.Dashboard;

public class TenantDashboardDto
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalActiveCourses { get; set; }
    public decimal TotalPayments { get; set; }
    public decimal MonthlyPayments { get; set; }
    public List<string> TodayEvents { get; set; }
    public List<string> ImportantNotifications { get; set; }
    public List<StudentDto> RecentStudents { get; set; }
    public List<MostActiveCourseDto> MostActiveCourses { get; set; }
    public List<CategoryPaymentStatDto> CategoryPaymentStats { get; set; }
    public decimal Last7DaysPayments { get; set; }
    public List<OverdueStudentDto> OverdueStudents { get; set; }
}

public class StudentDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime EnrollmentDate { get; set; }
}

public class MostActiveCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; }
    public int StudentCount { get; set; }
}

public class CategoryPaymentStatDto
{
    public int CategoryId { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OverdueStudentDto
{
    public Guid StudentId { get; set; }
    public string FullName { get; set; }
    public string CourseName { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public int DaysOverdue { get; set; }
}

