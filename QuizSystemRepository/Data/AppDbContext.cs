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
        public DbSet<ProfileUpdateRequest> ProfileUpdateRequests { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<QuestionBank>()
                .HasOne(qb => qb.Quiz)
                .WithMany(q => q.Questions)
                .HasForeignKey(qb => qb.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.Quiz)
                .WithMany()                        
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<QuizAttempt>()
                .HasOne(qa => qa.StudentUser)
                .WithMany()                        
                .HasForeignKey(qa => qa.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<AttemptedQuizAnswer>()
                .HasOne(a => a.QuizAttempt)
                .WithMany(qa => qa.Answers)
                .HasForeignKey(a => a.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<AttemptedQuizAnswer>()
                .HasOne(a => a.QuestionBank)
                .WithMany()                         
                .HasForeignKey(a => a.QuestionBankId)
                .OnDelete(DeleteBehavior.Restrict);
           

            modelBuilder.Entity<Class>()
                .Property(c => c.Name)
                .HasConversion<string>();

 
        }

    }

}
