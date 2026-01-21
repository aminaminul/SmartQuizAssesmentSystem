using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class InstructorRepository : IInstructorRepository
    {
        private readonly AppDbContext _context;

        public InstructorRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<Instructor>> GetAllAsync(long? educationMediumId = null, long? classId = null)
        {
            var query = _context.Instructor
                .Include(i => i.EducationMedium)
                .Include(i => i.Class)
                .Where(i => i.Status != ModelStatus.Deleted);

            if (educationMediumId.HasValue)
                query = query.Where(i => i.EducationMediumId == educationMediumId.Value);

            if (classId.HasValue)
                query = query.Where(i => i.ClassId == classId.Value);

            return query.ToListAsync();
        }

        public Task<Instructor?> GetByIdAsync(long id)
        {
            return _context.Instructor
                .Include(i => i.EducationMedium)
                .Include(i => i.Class)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<bool> EmailExistsAsync(string email, long? excludeId = null)
        {
            var query = _context.Instructor
                .Where(i => i.Email != null && i.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(i => i.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public Task<bool> PhoneExistsAsync(string phone, long? excludeId = null)
        {
            var query = _context.Instructor
                .Where(i => i.PhoneNumber != null && i.PhoneNumber == phone);

            if (excludeId.HasValue)
                query = query.Where(i => i.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public async Task AddAsync(Instructor instructor)
        {
            _context.Instructor.Add(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Instructor instructor)
        {
            _context.Instructor.Update(instructor);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        { 
             await _context.SaveChangesAsync(); 
        }

        public Task<List<EducationMedium>> GetEducationMediumsAsync()
        {
            return _context.EducationMedium.ToListAsync();
        }

        public Task<List<Class>> GetClassesAsync()
        {
            return _context.Class.ToListAsync();
        }
        public Task<Instructor?> GetByUserIdAsync(long userId)
        {
            return _context.Instructor
                .Include(i => i.User)
                .Include(i => i.Class)
                .Include(i => i.EducationMedium)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }
        public Task<List<Instructor>> GetPendingAsync()
        {
            return _context.Instructor
                .Include(i => i.EducationMedium)
                .Include(i => i.Class)
                .Where(i => i.Status == ModelStatus.Pending || i.Status == ModelStatus.InActive)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();
        }
    }
}
