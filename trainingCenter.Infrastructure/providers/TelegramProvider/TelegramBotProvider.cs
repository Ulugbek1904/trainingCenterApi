using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using trainingCenter.Common.Exceptions;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;

namespace trainingCenter.Infrastructure.providers.TelegramProvider;

public class TelegramBotProvider : ITelegramBotProvider
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<TelegramBotProvider> logger;
    private readonly Dictionary<Guid, TelegramBotClient> botClients = new();

    public TelegramBotProvider(
        IServiceScopeFactory scopeFactory,
        ILogger<TelegramBotProvider> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    public async Task SendMessageAsync(Guid tenantId, string chatId, string message, ReplyMarkup replyMarkup = null)
    {
        try
        {
            var client = await GetBotClientAsync(tenantId);
            await client.SendMessage(
                chatId: chatId,
                text: message,
                replyMarkup: replyMarkup);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"[Bot] Xabar yuborishda xatolik. TenantId: {tenantId}, ChatId: {chatId}");
            throw;
        }
    }

    public async Task<Dictionary<Guid, TelegramBotClient>> StartAllBotsAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var storageBroker = scope.ServiceProvider.GetRequiredService<IStorageBroker>();

        var botClientsMap = new Dictionary<Guid, TelegramBotClient>();

        var botSettings = await storageBroker.SelectAll<TelegramBotSetting>()
            .Include(x => x.Tenant)
            .ToListAsync(cancellationToken);

        logger.LogInformation($"[Bot] Found {botSettings.Count} bot settings in the database.");

        foreach (var setting in botSettings)
        {
            if (string.IsNullOrWhiteSpace(setting.BotToken))
            {
                logger.LogWarning($"[Bot] Skipping bot for TenantId: {setting.TenantId} due to empty or null token.");
                continue;
            }

            try
            {
                var client = new TelegramBotClient(setting.BotToken);
                var botInfo = await client.GetMe(cancellationToken);
                botClientsMap[setting.TenantId] = client;
                botClients[setting.TenantId] = client;
                logger.LogInformation($"[Bot] ✅ Bot initialized successfully. TenantId: {setting.TenantId}, Username: @{botInfo.Username}, Tenant: {setting.Tenant?.Name}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"[Bot] Failed to initialize bot for TenantId: {setting.TenantId}. Token: {setting.BotToken.Substring(0, 10)}...");
            }
        }

        return botClientsMap;
    }

    private async Task<TelegramBotClient> GetBotClientAsync(Guid tenantId)
    {
        if (botClients.TryGetValue(tenantId, out var client))
            return client;

        using var scope = scopeFactory.CreateScope();
        var storageBroker = scope.ServiceProvider.GetRequiredService<IStorageBroker>();

        var setting = await storageBroker.SelectAll<TelegramBotSetting>()
            .FirstOrDefaultAsync(s => s.TenantId == tenantId);

        if (setting == null || string.IsNullOrWhiteSpace(setting.BotToken))
            throw new NotFoundException($"Bot token for tenant {tenantId} not found.");

        client = new TelegramBotClient(setting.BotToken);
        botClients[tenantId] = client;

        return client;
    }

    private Task HandleErrorAsync(Guid tenantId, Exception exception)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiEx => $"[Bot] Telegram API xatosi: [{apiEx.ErrorCode}] {apiEx.Message}",
            _ => $"[Bot] Noma’lum xato: {exception.Message}"
        };

        logger.LogError(exception, $"[Bot] ❌ Polling xatosi (TenantId: {tenantId}) - {errorMessage}");
        return Task.CompletedTask;
    }
}
