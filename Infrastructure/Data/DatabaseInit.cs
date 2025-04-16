using Microsoft.Extensions.DependencyInjection;
using Shop.Core.Domain.Entities;
using Shop.Core.Domain.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Infrastructure.Data
{
    public static class DatabaseInit
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            if (!context.Users.Any(u => u.Role == UserRole.Admin))
            {
                var admin = new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    PasswordHash = passwordHasher.HashPassword("root123"),
                    Role = UserRole.Admin,
                    IsActivated = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userRepository.AddUserAsync(admin);
                Console.WriteLine("Администратор успешно добавлен в систему.");
            }
            else
            {
                Console.WriteLine("Администратор уже существует в системе.");
            }
        }
    }
}
