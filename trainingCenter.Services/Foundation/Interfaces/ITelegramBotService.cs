namespace trainingCenter.Services.Foundation.Interfaces;

public interface ITelegramBotService
{
    Task RegisterParentAsync(string parentTelegramId, string parentPhoneNumber);
    Task SendMessageToParentAsync(Guid studentId, string message);
    Task SendMessageToAllParentsAsync(string message);
    Task SendReportMenuAsync(string parentTelegramId);
    Task SendReportOptionsAsync(string parentTelegramId, Guid studentId);
    Task SendReportAsync(string parentTelegramId, string callbackData);
}
