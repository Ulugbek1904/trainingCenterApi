using Microsoft.EntityFrameworkCore;
using Serilog;
using trainingCenter.Infrastructure.brokers.storage;
using trainingCenter.Infrastructure.brokers.storage.Seed;

namespace trainingCenterApi.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<StorageBroker>(options =>
            {
                options.UseNpgsql(builder.Configuration
                    .GetConnectionString("DefaultConnection"));
            });

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .MinimumLevel.Debug();
            });

            builder.Services.AddLogging(logging => logging.AddSerilog());

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            await AppDbInitializier.SeedSuperAdminAsync(app.Services);

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
