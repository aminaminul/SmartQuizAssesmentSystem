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
        private readonly AppDbContext _context;
        private readonly RoleManager<QuizSystemRole> _roleManager;
        private readonly UserManager<QuizSystemUser> _userManager;
        private readonly ILogger<SeedService> _logger;

        public SeedService(AppDbContext context,RoleManager<QuizSystemRole> roleManager,UserManager<QuizSystemUser> userManager,ILogger<SeedService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }
        public void SeedDatabase()
        {
            try
            {
                _context.Database.Migrate();

                
                string[] roles = { "Admin", "Instructor", "Student" };
                foreach (var role in roles)
                {
                    if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        _roleManager.CreateAsync(new QuizSystemRole { Name = role })
                                   .GetAwaiter().GetResult();
                    }
                }

                
                var adminEmail = "admin@gmail.com";
                if (_userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult() == null)
                {
                    var admin = new QuizSystemUser
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = _userManager.CreateAsync(admin, "Admin@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(admin, "Admin")
                                   .GetAwaiter().GetResult();
                    }
                }

                
                var instructorEmail = "instructor@gmail.com";
                if (_userManager.FindByEmailAsync(instructorEmail).GetAwaiter().GetResult() == null)
                {
                    var instructor = new QuizSystemUser
                    {
                        FirstName = "Instructor",
                        LastName = "Test",
                        UserName = instructorEmail,
                        Email = instructorEmail,
                        EmailConfirmed = true
                    };

                    var result = _userManager.CreateAsync(instructor, "Instructor@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                        _userManager.AddToRoleAsync(instructor, "Instructor")
                                   .GetAwaiter().GetResult();
                    }
                }

                
                var studentEmail = "student@gmail.com";
                if (_userManager.FindByEmailAsync(studentEmail).GetAwaiter().GetResult() == null)
                {
                    var student = new QuizSystemUser
                    {
                        FirstName = "Student",
                        LastName = "Test",
                        UserName = studentEmail,
                        Email = studentEmail,
                        EmailConfirmed = true
                    };

                    var result = _userManager.CreateAsync(student, "Student@123")
                                           .GetAwaiter().GetResult();
                    if (result.Succeeded)
                    {
                         _userManager.AddToRoleAsync(student, "Student")
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
