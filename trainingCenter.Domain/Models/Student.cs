using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models;

public class Student
{
    public Guid Id { get; set; }
    [Phone]
    [Required, StringLength(100)]
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string ParentPhoneNumber { get; set; }
    public string? ParentTelegramId { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public List<StudentCourse> StudentCourses { get; set; } 
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
}
