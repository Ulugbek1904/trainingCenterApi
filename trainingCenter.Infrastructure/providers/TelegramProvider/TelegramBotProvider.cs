using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Common.Exceptions;
using trainingCenter.Infrastructure.providers.TelegramProvider;

public class TelegramBotProvider : ITelegramBotProvider
{
    private readonly TelegramBotClient botClient;
    private readonly ILogger<TelegramBotProvider> logger;

    public TelegramBotProvider(
        IConfiguration configuration,
        ILogger<TelegramBotProvider> logger
        )
    {
        var token = configuration["Telegram:BotToken"];
        if (string.IsNullOrEmpty(token))
            throw new ConfigurationException("Telegram bot token is missing in configuration.");
        botClient = new TelegramBotClient(token);
        this.logger = logger;
    }

    public async Task SendMessageAsync(string chatId, string message, ReplyMarkup replyMarkup = null)
    {
        await botClient.SendMessage(
            chatId: chatId,
            text: message,
            replyMarkup: replyMarkup);
    }

    public async Task StartReceivingAsync(Func<Update, Task> updateHandler, CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        await botClient.ReceiveAsync(
            updateHandler: async (client, update, ct) => await updateHandler(update),
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken);
    }

    private Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
    {
        this.logger.LogError($"Polling error: {exception.Message}");
        return Task.CompletedTask;
    }
}