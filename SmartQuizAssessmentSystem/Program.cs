using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;
using QuizSystemRepository.Interfaces;
using QuizSystemRepository.Repositories;
using QuizSystemService.Interfaces;
using QuizSystemService.Services;
using SmartQuizAssessmentSystem.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(conn));

// Identity
builder.Services.AddIdentity<QuizSystemUser, QuizSystemRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 3;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISeedService, SeedService>();

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<IInstructorRepository, InstructorRepository>();
builder.Services.AddScoped<IInstructorService, InstructorService>();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IEducationMediumRepository, EducationMediumRepository>();
builder.Services.AddScoped<IEducationMediumService, EducationMediumService>();

builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<IClassService, ClassService>();

builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IQuizService, QuizService>();

builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

builder.Services.AddScoped<IQuizAttemptRepository, QuizAttemptRepository>();
builder.Services.AddScoped<IAttemptedQuizAnswerRepository, AttemptedQuizAnswerRepository>();


builder.Services.AddScoped<IStudentDashboardRepository, StudentDashboardRepository>();
builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
builder.Services.AddScoped<IStudentQuizService, StudentQuizService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<ISeedService>();
    seeder.SeedDatabase();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();