using Telegram.Bot.Types;

namespace trainingCenter.Services.Orchestration.Interfaces;

public interface ITelegramBotOrchestration
{
    Task StartAsync(CancellationToken cancellationToken);
    Task ProcessUpdateAsync(Update update, Guid tenantId);
}
