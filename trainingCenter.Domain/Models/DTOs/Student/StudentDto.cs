namespace trainingCenter.Domain.Models.DTOs.Student;

public class StudentDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string ParentPhoneNumber { get; set; }
    public string ParentTelegramId { get; set; }
    public DateTime BirthDate { get; set; }
    public string Address { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
    public string Notes { get; set; }
    public List<StudentCourse> StudentCourses { get; set; }
}
