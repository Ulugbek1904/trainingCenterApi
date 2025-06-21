using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models;

public class Attendance
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public DateTime Date { get; set; }
    public Status Status { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
