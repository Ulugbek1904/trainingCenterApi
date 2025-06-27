using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Services.Foundation.Interfaces;

namespace trainingCenter.Services.Background
{
    public class PaymentReminderService : BackgroundService
    {
        private readonly IServiceProvider serviceProvider;

        public PaymentReminderService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceProvider.CreateScope();
                var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();
                var courses = await scope.ServiceProvider.GetRequiredService<IStorageBroker>()
                    .SelectAll<Course>()
                    .ToListAsync(stoppingToken);

                foreach (var course in courses)
                {
                    await paymentService.NotifyPendingPaymentsAsync(course.Id);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); 
            }
        }
    }
}