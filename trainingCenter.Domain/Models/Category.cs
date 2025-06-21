namespace trainingCenter.Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<Course> Courses { get; set; } = new List<Course>();
}
