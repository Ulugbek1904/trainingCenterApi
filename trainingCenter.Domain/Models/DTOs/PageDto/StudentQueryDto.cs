using trainingCenter.Domain.Enums;

namespace trainingCenter.Domain.Models.DTOs.PageDto;

public class StudentQueryDto
{
    public string? FullName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? StartYear { get; set; } 
    public DateTime? EndYear { get; set; }
    public string? Address { get; set; }
}
