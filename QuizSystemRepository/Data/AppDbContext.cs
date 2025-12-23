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

        public DbSet<EducationMediumModel> EducationMediums { get; set; }
        public DbSet<ClassModel> Classes { get; set; }
        public DbSet<QuizModel> Quiz { get; set; }
        public DbSet<SubjectModel> Subjects { get; set; }
        public DbSet<QuestionBankModel> QuestionBanks { get; set; }
        public DbSet<QuizAttemptModel> QuizAttempts { get; set; }
    }

}
