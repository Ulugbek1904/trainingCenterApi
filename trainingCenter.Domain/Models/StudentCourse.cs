namespace trainingCenter.Domain.Models;

public class StudentCourse
{
    public Guid StudentId { get; set; } 
    public Student Student { get; set; }
    public Guid CourseId { get; set; } 
    public Course Course { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public string GroupName { get; set; } = string.Empty; 
    public bool IsActive { get; set; } 
                                       
    public decimal? Discount { get; set; } 
    public string Notes { get; set; } = string.Empty;
}
