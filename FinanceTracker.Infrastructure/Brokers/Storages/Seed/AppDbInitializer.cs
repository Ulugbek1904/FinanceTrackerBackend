using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Infrastructure.Brokers.Storages.Seed
{
    public static class AppDbInitializer
    {
        // serviceProvider - bu service'larni olish uchun ishlatiladigan ob'ekt
        // CreateScope - bu serviceProvider'dan yangi scope yaratadi
        // GetRequiredService - bu scope ichidan kerakli service'ni olish uchun ishlatiladi
        // IServiceCollection - bu service'larni ro'yxatga olish uchun ishlatiladigan interfeys
        public static async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StorageBroker>();

            var superAdminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var superAdminEmail = "julugbek023@gmail.com";

            if (!await dbContext.Users.AnyAsync(u => u.Id == superAdminId))
            {
                var superAdmin = new User
                {
                    Id = superAdminId,
                    Email = superAdminEmail,
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("Qwerty1904"),
                    FirstName = "Super",
                    LastName = "Admin",
                    Role = Role.SuperAdmin,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await dbContext.Users.AddAsync(superAdmin);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
