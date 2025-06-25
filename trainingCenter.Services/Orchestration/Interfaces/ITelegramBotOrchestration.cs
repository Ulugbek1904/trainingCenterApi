namespace trainingCenter.Services.Orchestration.Interfaces;

public interface ITelegramBotOrchestration
{
    Task ProcessUpdateAsync(Telegram.Bot.Types.Update update);
    Task StartAsync(CancellationToken cancellationToken);
}
