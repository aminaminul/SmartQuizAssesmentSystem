using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuizSystemModel.BusinessRules;
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

        public SeedService(
            AppDbContext context,
            RoleManager<QuizSystemRole> roleManager,
            UserManager<QuizSystemUser> userManager,
            ILogger<SeedService> logger)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public void SeedDatabase()
        {
            SeedDatabaseAsync().GetAwaiter().GetResult();
        }

        public async Task SeedDatabaseAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();

                // 1. Seed Roles
                string[] roles = { "Admin", "Instructor", "Student" };
                foreach (var role in roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new QuizSystemRole { Name = role });
                    }
                }

                // 2. Seed Default Education Medium
                var defaultMedium = await _context.EducationMedium.FirstOrDefaultAsync(m => m.Name == "Bangla Medium");
                if (defaultMedium == null)
                {
                    defaultMedium = new EducationMedium
                    {
                        Name = "Bangla Medium",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.EducationMedium.AddAsync(defaultMedium);
                    await _context.SaveChangesAsync();
                }

                var englishMedium = await _context.EducationMedium.FirstOrDefaultAsync(m => m.Name == "English Medium");
                if (englishMedium == null)
                {
                    englishMedium = new EducationMedium
                    {
                        Name = "English Medium",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.EducationMedium.AddAsync(englishMedium);
                    await _context.SaveChangesAsync();
                }

                // 3. Seed Default Class
                var defaultClass = await _context.Class.FirstOrDefaultAsync(c => c.Name == "Class 10" && c.EducationMediumId == defaultMedium.Id);
                if (defaultClass == null)
                {
                    defaultClass = new Class
                    {
                        Name = "Class 10",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        EducationMediumId = defaultMedium.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Class.AddAsync(defaultClass);
                    await _context.SaveChangesAsync();
                }

                var class9 = await _context.Class.FirstOrDefaultAsync(c => c.Name == "Class 9" && c.EducationMediumId == defaultMedium.Id);
                if (class9 == null)
                {
                    class9 = new Class
                    {
                        Name = "Class 9",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        EducationMediumId = defaultMedium.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Class.AddAsync(class9);
                    await _context.SaveChangesAsync();
                }

                // 4. Seed Default Subject
                var defaultSubject = await _context.Subject.FirstOrDefaultAsync(s => s.Name == "Math" && s.ClassId == defaultClass.Id);
                if (defaultSubject == null)
                {
                    defaultSubject = new Subject
                    {
                        Name = "Math",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        ClassId = defaultClass.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Subject.AddAsync(defaultSubject);
                    await _context.SaveChangesAsync();
                }

                var banglaSubject = await _context.Subject.FirstOrDefaultAsync(s => s.Name == "Bangla" && s.ClassId == defaultClass.Id);
                if (banglaSubject == null)
                {
                    banglaSubject = new Subject
                    {
                        Name = "Bangla",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        ClassId = defaultClass.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Subject.AddAsync(banglaSubject);
                    await _context.SaveChangesAsync();
                }

                var englishSubject = await _context.Subject.FirstOrDefaultAsync(s => s.Name == "English" && s.ClassId == defaultClass.Id);
                if (englishSubject == null)
                {
                    englishSubject = new Subject
                    {
                        Name = "English",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        ClassId = defaultClass.Id,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Subject.AddAsync(englishSubject);
                    await _context.SaveChangesAsync();
                }

                // 5. Seed / Sync Admin User
                var adminEmail = "admin@gmail.com";
                var admin = await _userManager.FindByEmailAsync(adminEmail);
                if (admin == null)
                {
                    admin = new QuizSystemUser
                    {
                        FirstName = "Admin",
                        LastName = "User",
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };
                    var createRes = await _userManager.CreateAsync(admin, "Admin@123");
                    if (createRes.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
                else
                {
                    // Ensure role and password match Admin@123
                    if (!await _userManager.IsInRoleAsync(admin, "Admin"))
                    {
                        await _userManager.AddToRoleAsync(admin, "Admin");
                    }
                    var checkPass = await _userManager.CheckPasswordAsync(admin, "Admin@123");
                    if (!checkPass)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
                        await _userManager.ResetPasswordAsync(admin, token, "Admin@123");
                    }
                }

                // 6. Seed / Sync Instructor User & Instructor Record
                var instructorEmail = "instructor@gmail.com";
                var instructor = await _userManager.FindByEmailAsync(instructorEmail);
                if (instructor == null)
                {
                    instructor = new QuizSystemUser
                    {
                        FirstName = "Demo",
                        LastName = "Instructor",
                        UserName = instructorEmail,
                        Email = instructorEmail,
                        EmailConfirmed = true
                    };
                    var createRes = await _userManager.CreateAsync(instructor, "Instructor@123");
                    if (createRes.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(instructor, "Instructor");
                    }
                }
                else
                {
                    if (!await _userManager.IsInRoleAsync(instructor, "Instructor"))
                    {
                        await _userManager.AddToRoleAsync(instructor, "Instructor");
                    }
                    var checkPass = await _userManager.CheckPasswordAsync(instructor, "Instructor@123");
                    if (!checkPass)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(instructor);
                        await _userManager.ResetPasswordAsync(instructor, token, "Instructor@123");
                    }
                }

                // Ensure Instructor Table Entry exists and is Active
                if (instructor != null)
                {
                    var instructorRecord = await _context.Instructor.FirstOrDefaultAsync(i => i.UserId == instructor.Id || i.Email == instructorEmail);
                    if (instructorRecord == null)
                    {
                        instructorRecord = new Instructor
                        {
                            FirstName = "Demo",
                            LastName = "Instructor",
                            Email = instructorEmail,
                            PhoneNumber = "01700000002",
                            UserId = instructor.Id,
                            Status = ModelStatus.Active,
                            EducationMediumId = defaultMedium.Id,
                            ClassId = defaultClass.Id,
                            SubjectId = defaultSubject.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _context.Instructor.AddAsync(instructorRecord);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        instructorRecord.Status = ModelStatus.Active;
                        instructorRecord.UserId = instructor.Id;
                        if (!instructorRecord.EducationMediumId.HasValue) instructorRecord.EducationMediumId = defaultMedium.Id;
                        if (!instructorRecord.ClassId.HasValue) instructorRecord.ClassId = defaultClass.Id;
                        if (!instructorRecord.SubjectId.HasValue) instructorRecord.SubjectId = defaultSubject.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                // 7. Seed / Sync Student User & Student Record
                var studentEmail = "student@gmail.com";
                var student = await _userManager.FindByEmailAsync(studentEmail);
                if (student == null)
                {
                    student = new QuizSystemUser
                    {
                        FirstName = "Demo",
                        LastName = "Student",
                        UserName = studentEmail,
                        Email = studentEmail,
                        EmailConfirmed = true
                    };
                    var createRes = await _userManager.CreateAsync(student, "Student@123");
                    if (createRes.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(student, "Student");
                    }
                }
                else
                {
                    if (!await _userManager.IsInRoleAsync(student, "Student"))
                    {
                        await _userManager.AddToRoleAsync(student, "Student");
                    }
                    var checkPass = await _userManager.CheckPasswordAsync(student, "Student@123");
                    if (!checkPass)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(student);
                        await _userManager.ResetPasswordAsync(student, token, "Student@123");
                    }
                }

                // Ensure Student Table Entry exists and is Active
                if (student != null)
                {
                    var studentRecord = await _context.Student.FirstOrDefaultAsync(s => s.UserId == student.Id || s.Email == studentEmail);
                    if (studentRecord == null)
                    {
                        studentRecord = new Student
                        {
                            FirstName = "Demo",
                            LastName = "Student",
                            Email = studentEmail,
                            PhoneNumber = "01700000003",
                            UserId = student.Id,
                            Status = ModelStatus.Active,
                            EducationMediumId = defaultMedium.Id,
                            ClassId = defaultClass.Id,
                            CreatedAt = DateTime.UtcNow
                        };
                        await _context.Student.AddAsync(studentRecord);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        studentRecord.Status = ModelStatus.Active;
                        studentRecord.UserId = student.Id;
                        if (!studentRecord.EducationMediumId.HasValue) studentRecord.EducationMediumId = defaultMedium.Id;
                        if (!studentRecord.ClassId.HasValue) studentRecord.ClassId = defaultClass.Id;
                        await _context.SaveChangesAsync();
                    }
                }

                _logger.LogInformation("Database seeded successfully with Admin, Instructor, and Student accounts and related data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database seeding failed");
            }
        }
    }
}
