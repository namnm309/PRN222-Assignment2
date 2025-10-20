using System;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Seed
{
    public static class SeedDefaults
    {
        public static async Task EnsureEvmStaffAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;
            var context = provider.GetRequiredService<AppDbContext>();
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedDefaults");

            await context.Database.MigrateAsync();

            var evmStaffEmail = "evm.staff@vinfast.com";
            var existingEVMStaff = await context.Users.FirstOrDefaultAsync(u => u.Email == evmStaffEmail);
            if (existingEVMStaff != null)
            {
                logger.LogInformation("EVMStaff exists: {Email}", evmStaffEmail);
                return;
            }

            var evmStaffUser = new Users
            {
                Id = Guid.NewGuid(),
                FullName = "EVM Staff",
                Email = evmStaffEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("evmstaff123"),
                PhoneNumber = "0900000001",
                Address = "VinFast HQ, Hanoi",
                Role = DataAccessLayer.Enum.UserRole.EVMStaff,
                DealerId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.Users.Add(evmStaffUser);
            await context.SaveChangesAsync();
            logger.LogInformation("Created default EVMStaff: {Email}", evmStaffEmail);
        }
    }
}


