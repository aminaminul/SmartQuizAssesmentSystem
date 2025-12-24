using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class SeedService : ISeedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SeedService> _logger;

        public SeedService(IServiceProvider serviceProvider, ILogger<SeedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void SeedDatabase()
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<QuizSystemRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<QuizSystemUser>>();

            try
            {
                // Migration (sync)
                context.Database.Migrate();

                // Roles
                string[] roles = { "Admin", "Instructor", "Student" };
                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        roleManager.CreateAsync(new QuizSystemRole { Name = role })
                                   .GetAwaiter().GetResult();
                    }
                }

                // Admin
                var adminEmail = "admin@gmail.com";
                if (userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult() == null)
                {
                    var admin = new QuizSystemUser
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(admin, "Admin@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(admin, "Admin")
                                   .GetAwaiter().GetResult();
                    }
                }

                // Instructor
                var instructorEmail = "instructor@gmail.com";
                if (userManager.FindByEmailAsync(instructorEmail).GetAwaiter().GetResult() == null)
                {
                    var instructor = new QuizSystemUser
                    {
                        FirstName = "Instructor",
                        LastName = "Test",
                        UserName = instructorEmail,
                        Email = instructorEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(instructor, "Instructor@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(instructor, "Instructor")
                                   .GetAwaiter().GetResult();
                    }
                }

                // Student
                var studentEmail = "student@gmail.com";
                if (userManager.FindByEmailAsync(studentEmail).GetAwaiter().GetResult() == null)
                {
                    var student = new QuizSystemUser
                    {
                        FirstName = "Student",
                        LastName = "Test",
                        UserName = studentEmail,
                        Email = studentEmail,
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(student, "Student@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(student, "Student")
                                   .GetAwaiter().GetResult();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database seeding failed");
            }
        }
    }
}
