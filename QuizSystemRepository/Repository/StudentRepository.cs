using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Student>> GetAllAsync()
        {
            return _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .Where(s => s.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

        public Task<Student?> GetByIdAsync(long id)
        {
            return _context.Student
                .Include(s => s.EducationMedium)
                .Include(s => s.Class)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<bool> EmailExistsAsync(string email, long? excludeId = null)
        {
            var query = _context.Student
                .Where(s => s.Email != null && s.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public Task<bool> PhoneExistsAsync(string phone, long? excludeId = null)
        {
            var query = _context.Student
                .Where(s => s.PhoneNumber != null && s.PhoneNumber == phone);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public async Task AddAsync(Student student)
        {
            _context.Student.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Student.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task<List<EducationMedium>> GetEducationMediumsAsync()
        {
            return _context.EducationMedium
                .Where(m => m.Status != ModelStatus.Deleted)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public Task<List<Class>> GetClassesAsync(long? mediumId = null)
        {
            var query = _context.Class
                .Where(c => c.Status != ModelStatus.Deleted);

            if (mediumId.HasValue)
            {
                query = query.Where(c => c.EducationMediumId == mediumId.Value);
            }

            return query
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public Task<Student?> GetByUserIdAsync(long userId)
        {
            return _context.Student
                .Include(s => s.User)
                .Include(s => s.Class)
                .Include(s => s.EducationMedium)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}
