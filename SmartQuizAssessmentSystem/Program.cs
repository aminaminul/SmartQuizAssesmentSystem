using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizSystemRepository.Data;
using QuizSystemModel.Models;
using QuizSystemService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(conn));

//builder.Services.AddDbContext<AppDbContext>(options=>
//.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<QuizSystemUser, QuizSystemRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

await SeedService.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();


app.UseAuthentication();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
