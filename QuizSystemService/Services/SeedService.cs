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
            try
            {
                _context.Database.Migrate();

                // 1. Seed Roles
                string[] roles = { "Admin", "Instructor", "Student" };
                foreach (var role in roles)
                {
                    if (!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                    {
                        _roleManager.CreateAsync(new QuizSystemRole { Name = role })
                                   .GetAwaiter().GetResult();
                    }
                }

                // 2. Seed Default Education Medium
                var defaultMedium = _context.EducationMedium.FirstOrDefault(m => m.Name == "Bangla Medium");
                if (defaultMedium == null)
                {
                    defaultMedium = new EducationMedium
                    {
                        Name = "Bangla Medium",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.EducationMedium.Add(defaultMedium);
                    _context.SaveChanges();
                }

                var englishMedium = _context.EducationMedium.FirstOrDefault(m => m.Name == "English Medium");
                if (englishMedium == null)
                {
                    englishMedium = new EducationMedium
                    {
                        Name = "English Medium",
                        IsApproved = true,
                        Status = ModelStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.EducationMedium.Add(englishMedium);
                    _context.SaveChanges();
                }

                // 3. Seed Default Class
                var defaultClass = _context.Class.FirstOrDefault(c => c.Name == "Class 10" && c.EducationMediumId == defaultMedium.Id);
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
                    _context.Class.Add(defaultClass);
                    _context.SaveChanges();
                }

                var class9 = _context.Class.FirstOrDefault(c => c.Name == "Class 9" && c.EducationMediumId == defaultMedium.Id);
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
                    _context.Class.Add(class9);
                    _context.SaveChanges();
                }

                // 4. Seed Default Subject
                var defaultSubject = _context.Subject.FirstOrDefault(s => s.Name == "Math" && s.ClassId == defaultClass.Id);
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
                    _context.Subject.Add(defaultSubject);
                    _context.SaveChanges();
                }

                var banglaSubject = _context.Subject.FirstOrDefault(s => s.Name == "Bangla" && s.ClassId == defaultClass.Id);
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
                    _context.Subject.Add(banglaSubject);
                    _context.SaveChanges();
                }

                var englishSubject = _context.Subject.FirstOrDefault(s => s.Name == "English" && s.ClassId == defaultClass.Id);
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
                    _context.Subject.Add(englishSubject);
                    _context.SaveChanges();
                }

                // 5. Seed / Sync Admin User
                var adminEmail = "admin@gmail.com";
                var admin = _userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
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
                    var createRes = _userManager.CreateAsync(admin, "Admin@123").GetAwaiter().GetResult();
                    if (createRes.Succeeded)
                    {
                        _userManager.AddToRoleAsync(admin, "Admin").GetAwaiter().GetResult();
                    }
                }
                else
                {
                    // Ensure role and password match Admin@123
                    if (!_userManager.IsInRoleAsync(admin, "Admin").GetAwaiter().GetResult())
                    {
                        _userManager.AddToRoleAsync(admin, "Admin").GetAwaiter().GetResult();
                    }
                    var checkPass = _userManager.CheckPasswordAsync(admin, "Admin@123").GetAwaiter().GetResult();
                    if (!checkPass)
                    {
                        var token = _userManager.GeneratePasswordResetTokenAsync(admin).GetAwaiter().GetResult();
                        _userManager.ResetPasswordAsync(admin, token, "Admin@123").GetAwaiter().GetResult();
                    }
                }

                // 6. Seed / Sync Instructor User & Instructor Record
                var instructorEmail = "instructor@gmail.com";
                var instructor = _userManager.FindByEmailAsync(instructorEmail).GetAwaiter().GetResult();
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
                    var createRes = _userManager.CreateAsync(instructor, "Instructor@123").GetAwaiter().GetResult();
                    if (createRes.Succeeded)
                    {
                        _userManager.AddToRoleAsync(instructor, "Instructor").GetAwaiter().GetResult();
                    }
                }
                else
                {
                    if (!_userManager.IsInRoleAsync(instructor, "Instructor").GetAwaiter().GetResult())
                    {
                        _userManager.AddToRoleAsync(instructor, "Instructor").GetAwaiter().GetResult();
                    }
                    var checkPass = _userManager.CheckPasswordAsync(instructor, "Instructor@123").GetAwaiter().GetResult();
                    if (!checkPass)
                    {
                        var token = _userManager.GeneratePasswordResetTokenAsync(instructor).GetAwaiter().GetResult();
                        _userManager.ResetPasswordAsync(instructor, token, "Instructor@123").GetAwaiter().GetResult();
                    }
                }

                // Ensure Instructor Table Entry exists and is Active
                if (instructor != null)
                {
                    var instructorRecord = _context.Instructor.FirstOrDefault(i => i.UserId == instructor.Id || i.Email == instructorEmail);
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
                        _context.Instructor.Add(instructorRecord);
                        _context.SaveChanges();
                    }
                    else
                    {
                        instructorRecord.Status = ModelStatus.Active;
                        instructorRecord.UserId = instructor.Id;
                        if (!instructorRecord.EducationMediumId.HasValue) instructorRecord.EducationMediumId = defaultMedium.Id;
                        if (!instructorRecord.ClassId.HasValue) instructorRecord.ClassId = defaultClass.Id;
                        if (!instructorRecord.SubjectId.HasValue) instructorRecord.SubjectId = defaultSubject.Id;
                        _context.SaveChanges();
                    }
                }

                // 7. Seed / Sync Student User & Student Record
                var studentEmail = "student@gmail.com";
                var student = _userManager.FindByEmailAsync(studentEmail).GetAwaiter().GetResult();
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
                    var createRes = _userManager.CreateAsync(student, "Student@123").GetAwaiter().GetResult();
                    if (createRes.Succeeded)
                    {
                        _userManager.AddToRoleAsync(student, "Student").GetAwaiter().GetResult();
                    }
                }
                else
                {
                    if (!_userManager.IsInRoleAsync(student, "Student").GetAwaiter().GetResult())
                    {
                        _userManager.AddToRoleAsync(student, "Student").GetAwaiter().GetResult();
                    }
                    var checkPass = _userManager.CheckPasswordAsync(student, "Student@123").GetAwaiter().GetResult();
                    if (!checkPass)
                    {
                        var token = _userManager.GeneratePasswordResetTokenAsync(student).GetAwaiter().GetResult();
                        _userManager.ResetPasswordAsync(student, token, "Student@123").GetAwaiter().GetResult();
                    }
                }

                // Ensure Student Table Entry exists and is Active
                if (student != null)
                {
                    var studentRecord = _context.Student.FirstOrDefault(s => s.UserId == student.Id || s.Email == studentEmail);
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
                        _context.Student.Add(studentRecord);
                        _context.SaveChanges();
                    }
                    else
                    {
                        studentRecord.Status = ModelStatus.Active;
                        studentRecord.UserId = student.Id;
                        if (!studentRecord.EducationMediumId.HasValue) studentRecord.EducationMediumId = defaultMedium.Id;
                        if (!studentRecord.ClassId.HasValue) studentRecord.ClassId = defaultClass.Id;
                        _context.SaveChanges();
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
