using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;
using trainingCenter.Infrastructure.brokers.storage;

public static class AppDbInitializier
{
    public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StorageBroker>();

        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // 1. Tenantni yaratish
        var superAdminTenant = await dbContext.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId);
        if (superAdminTenant == null)
        {
            superAdminTenant = new Tenant
            {
                Id = tenantId,
                Name = "SuperAdminTenant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await dbContext.Tenants.AddAsync(superAdminTenant);
            await dbContext.SaveChangesAsync();
        }

        // 2. TelegramBotSetting yaratish
        var existingBotSetting = await dbContext.Set<TelegramBotSetting>()
            .FirstOrDefaultAsync(b => b.TenantId == tenantId);

        if (existingBotSetting == null)
        {
            var botSetting = new TelegramBotSetting
            {
                TelegramBotId = Guid.NewGuid(),
                TenantId = tenantId,
                BotToken = "7896633633:AAGwgltJ9fXa4nZ9t5AwCcsRXgS7L3EMub0" 
            };

            await dbContext.Set<TelegramBotSetting>().AddAsync(botSetting);
            await dbContext.SaveChangesAsync();
        }

        // 3. SuperAdmin foydalanuvchi yaratish
        var existingUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == superAdminId);
        if (existingUser == null)
        {
            var superAdmin = new User
            {
                Id = superAdminId,
                TenantId = tenantId,
                Username = "superadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Qwerty1904"),
                Role = Role.SuperAdmin,
                FullName = "Super Admin",
                Email = "julugbek023@gmail.com",
                PhoneNumber = "+998940641904",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await dbContext.Users.AddAsync(superAdmin);
            await dbContext.SaveChangesAsync();
        }
    }
}
