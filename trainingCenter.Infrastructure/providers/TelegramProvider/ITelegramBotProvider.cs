using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace trainingCenter.Infrastructure.providers.TelegramProvider
{
    public interface ITelegramBotProvider
    {
        Task SendMessageAsync(string chatId, string message, ReplyMarkup replyMarkup = null);
        Task StartReceivingAsync(Func<Update, Task> updateHandler, CancellationToken cancellationToken);
    }
}