using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using trainingCenter.Domain.Enums;
using trainingCenter.Domain.Models;

namespace trainingCenter.Infrastructure.brokers.storage.Seed;

public static class AppDbInitializier
{
    public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
    {

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StorageBroker>();

        var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        if (!await dbContext.Users.AnyAsync(u => u.Id == superAdminId))
        {
            var superAdmin = new User
            {
                Id = superAdminId,
                Username = "superadmin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Qwerty1904"),
                Role = Role.Admin,
                FullName = "Super Admin",
                Email = "julugbek023@gmail.com",
                PhoneNumber = "+998940641904",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await dbContext.Users.AddAsync(superAdmin);
            await dbContext.SaveChangesAsync();
        }
    }
}
