using trainingCenter.Infrastructure.providers.AuthProvider;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;
using trainingCenter.Services.Orchestration;
using trainingCenter.Infrastructure.brokers.storage;

namespace trainingCenterApi.Presentation.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthProvider, AuthProvider>();
        services.AddScoped<ITelegramBotProvider, TelegramBotProvider>();
        services.AddScoped<ITelegramBotService, TelegramBotService>();
        services.AddScoped<ITelegramBotOrchestration, TelegramBotOrchestrator>();
        services.AddScoped<IStorageBroker, StorageBroker>();
        services.AddHostedService<BotBackgroundService>();


        return services;
    }
}

public class BotBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;

    public BotBackgroundService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var botOrchestrator = scope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();
        await botOrchestrator.StartAsync(stoppingToken);
    }
}