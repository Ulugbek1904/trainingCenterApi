using trainingCenter.Infrastructure.providers.AuthProvider;
using trainingCenter.Infrastructure.providers.TelegramProvider;
using trainingCenter.Services.Foundation;
using trainingCenter.Services.Foundation.Interfaces;
using trainingCenter.Services.Orchestration.Interfaces;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Common.Exceptions;
using trainingCenterApi.Presentation.Mappings;
using trainingCenter.Services.Background;
using trainingCenter.Services.Orchestration;

namespace trainingCenterApi.Presentation.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<IAuthProvider, AuthProvider>();
        services.AddScoped<ITelegramBotProvider, TelegramBotProvider>();
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



        return services;
    }
}

public class BotBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;

    public BotBackgroundService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory ?? throw new NullArgumentException(nameof(scopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();
        var botOrchestrator = scope.ServiceProvider.GetRequiredService<ITelegramBotOrchestration>();
        await botOrchestrator.StartAsync(stoppingToken);
    }
}