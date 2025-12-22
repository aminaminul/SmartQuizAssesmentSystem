using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuizSystemModel.Models;

namespace QuizSystemRepository.Data
{
    public class AppDbContext : IdentityDbContext<QuizSystemUser, QuizSystemRole, long>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
