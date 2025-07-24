namespace trainingCenter.Services.Foundation.Interfaces;

public interface ITelegramBotService
{
    Task RegisterParentAsync(string telegramId, string phoneNumber);
    Task SendMenuAsync(Guid tenantId, string telegramId);
    Task SendReportMenuAsync(Guid tenantId, string telegramId);
    Task SendReportOptionsAsync(Guid tenantId, string telegramId, Guid studentId);
    Task SendHelpAsync(Guid tenantId, string telegramId);
    Task HandleRegisterPhoneNumberAsync(Guid tenantId, string telegramId, string phoneNumber);
}
