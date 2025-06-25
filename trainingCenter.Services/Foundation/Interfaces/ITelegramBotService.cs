namespace trainingCenter.Services.Foundation.Interfaces;

public interface ITelegramBotService
{
    Task RegisterParentAsync(string parentTelegramId, string parentPhoneNumber);
    Task SendMessageToParentAsync(Guid studentId, string message);
    Task SendMessageToAllParentsAsync(string message);
}
