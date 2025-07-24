namespace trainingCenter.Domain.Models;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string ContactPhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public List<User> Users { get; set; }
    public List<Student> Students { get; set; }
    public List<Course> Courses { get; set; }

    public TelegramBotSetting TelegramBotSetting { get; set; }
}

