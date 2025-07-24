using trainingCenter.Infrastructure.providers.AuthProvider;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenterApi.Presentation.Mappings;
using trainingCenter.Services.Background;
using trainingCenter.Services.Orchestration;
using trainingCenterApi.Common.HelperFunctions;
using trainingCenter.Infrastructure.services;
using Telegram.Bot.Polling;
using Telegram.Bot;

namespace trainingCenterApi.Presentation.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthProvider, AuthProvider>();
        services.AddSingleton<ITelegramBotProvider, TelegramBotProvider>();
        services.AddScoped<ITelegramBotService, TelegramBotService>();
        services.AddScoped<ITelegramBotOrchestration, TelegramBotOrchestration>();
        services.AddScoped<IStorageBroker, StorageBroker>();
        services.AddHostedService<BotBackgroundService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IStudentCourseService, StudentCourseService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IGradeService, GradeService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddHostedService<PaymentReminderService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped< ITenantDashboardService, TenantDashboardService>();
        services.AddScoped<ISuperAdminDashboardService, SuperAdminDashboardService>();

        return services;
    }
}

public class BotBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly ILogger<BotBackgroundService> logger;

    public BotBackgroundService(IServiceScopeFactory scopeFactory, ILogger<BotBackgroundService> logger)
    {
        this.scopeFactory = scopeFactory;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[Bot] Starting BotBackgroundService...");
        using var scope = scopeFactory.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<ITelegramBotProvider>();
        var orchestration = scope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();

        try
        {
            var bots = await provider.StartAllBotsAsync(stoppingToken);
            logger.LogInformation($"[Bot] Initialized {bots.Count} bots.");

            foreach (var kvp in bots)
            {
                var tenantId = kvp.Key;
                var client = kvp.Value;

                try
                {
                    var botInfo = await client.GetMe(stoppingToken);
                    logger.LogInformation($"[Bot] Starting polling for TenantId: {tenantId}, Bot: @{botInfo.Username}");

                    client.StartReceiving(
                        updateHandler: async (bot, update, token) =>
                        {
                            try
                            {
                                using var innerScope = scopeFactory.CreateScope();
                                var scopedOrchestration = innerScope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();
                                await scopedOrchestration.ProcessUpdateAsync(update, tenantId);
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, $"[Bot] ❌ Orchestration error. TenantId: {tenantId}");
                            }
                        },
                        errorHandler: (bot, exception, token) =>
                        {
                            logger.LogError(exception, $"[Bot] ❌ Polling error. TenantId: {tenantId}");
                            return Task.CompletedTask;
                        },
                        receiverOptions: new ReceiverOptions(),
                        cancellationToken: stoppingToken
                    );
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"[Bot] Failed to start polling for TenantId: {tenantId}");
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[Bot] Failed to initialize bots in BotBackgroundService.");
        }
    }
}

//public class BotBackgroundService : BackgroundService
//{
//    private readonly IServiceScopeFactory scopeFactory;
//    private readonly ILogger<BotBackgroundService> logger;

//    public BotBackgroundService(IServiceScopeFactory scopeFactory, ILogger<BotBackgroundService> logger)
//    {
//        this.scopeFactory = scopeFactory;
//        this.logger = logger;
//    }

//    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//    {
//        using var scope = scopeFactory.CreateScope();
//        var provider = scope.ServiceProvider.GetRequiredService<ITelegramBotProvider>();
//        var orchestration = scope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();

//        var bots = await provider.StartAllBotsAsync(stoppingToken);

//        foreach (var kvp in bots)
//        {
//            var tenantId = kvp.Key;
//            var client = kvp.Value;

//            client.StartReceiving(
//                updateHandler: async (bot, update, token) =>
//                {
//                    try
//                    {
//                        using var innerScope = scopeFactory.CreateScope();
//                        var scopedOrchestration = innerScope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();
//                        await scopedOrchestration.ProcessUpdateAsync(update);
//                    }
//                    catch (Exception ex)
//                    {
//                        logger.LogError(ex, $"[Bot] ❌ Orchestration error. TenantId: {tenantId}");
//                    }
//                },
//                errorHandler: (bot, exception, token) =>
//                {
//                    logger.LogError(exception, $"[Bot] ❌ Polling error. TenantId: {tenantId}");
//                    return Task.CompletedTask;
//                },
//                receiverOptions: new ReceiverOptions(),
//                cancellationToken: stoppingToken
//            );
//        }
//    }
//}

