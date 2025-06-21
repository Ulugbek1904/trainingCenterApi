using System.ComponentModel.DataAnnotations;

namespace trainingCenter.Domain.Models;

public class Grade
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    [Range(0, 100)]
    public int Score { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime Date { get; set; } 
    public Guid TeacherId { get; set; }
    public User Teacher { get; set; }
}
