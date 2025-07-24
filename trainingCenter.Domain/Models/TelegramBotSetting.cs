namespace trainingCenter.Domain.Models;

public class TelegramBotSetting
{
    public Guid TelegramBotId { get; set; }
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public string BotToken { get; set; }
}