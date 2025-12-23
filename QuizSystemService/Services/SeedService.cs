using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemService.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<QuizSystemRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<QuizSystemUser>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                // ✅ Migration (NOT EnsureCreated)
                await context.Database.MigrateAsync();

                // ✅ Roles
                string[] roles = { "Admin", "Instructor", "Student" };
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new QuizSystemRole { Name = role });
                    }
                }

                // ✅ Admin User
                var adminEmail = "admin@gmail.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new QuizSystemUser
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database seeding failed");
            }
        }
    }
}
