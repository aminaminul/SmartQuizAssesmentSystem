using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartQuizAssessmentSystem.Models;

namespace SmartQuizAssessmentSystem.Data
{
    public class AppDbContext : IdentityDbContext<QuizSystemUser, QuizSystemRole, long>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
