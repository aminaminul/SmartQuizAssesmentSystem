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

        public DbSet<EducationMedium> EducationMedium { get; set; }
        public DbSet<Class> Class { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<Subject> Subject { get; set; }
        public DbSet<QuestionBank> QuestionBank { get; set; }
        public DbSet<QuizAttempt> QuizAttempt { get; set; }
        public DbSet<AttemptedQuizAnswer> AttemptedQuizAnswer { get; set; }
        public DbSet<Student> Student { get; set; }
        public DbSet<Instructor> Instructor { get; set; }
    }

}
