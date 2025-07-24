using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace trainingCenter.Infrastructure.providers.TelegramProvider
{
    public interface ITelegramBotProvider
    {
        Task SendMessageAsync(Guid tenantId, string chatId, string message, ReplyMarkup replyMarkup = null);
        Task<Dictionary<Guid, TelegramBotClient>> StartAllBotsAsync(CancellationToken cancellationToken);
    }
}