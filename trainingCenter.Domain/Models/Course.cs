using System.ComponentModel.DataAnnotations;
using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models;

public class Course
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; } 
    public DateTime StartDate { get; set; } = DateTime.UtcNow; 
    public DateTime? EndDate { get; set; } 
    public Guid TeacherId { get; set; } 
    public User Teacher { get; set; } 
    public int MaxStudents { get; set; } 
    public string Schedule { get; set; } = string.Empty; 
    public bool IsActive { get; set; } 
    public List<StudentCourse> Students { get; set; } = new List<StudentCourse>(); 
    public int? CategoryId { get; set; }
    public string Materials { get; set; } = string.Empty;
    public CourseLevel Level { get; set; }
}
