namespace trainingCenter.Domain.Models.DTOs.Tenant;

public class TenantUpdateDto
{
    public string Name { get; set; }
    public string LogoUrl { get; set; }
    public string TelegramBotToken { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string Address { get; set; }
    public string Language { get; set; }
    public bool IsActive { get; set; }
}

