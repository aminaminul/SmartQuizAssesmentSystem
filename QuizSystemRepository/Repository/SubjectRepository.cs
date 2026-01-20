using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllAsync(long? classId = null)
        {
            var query = _context.Subject
                .Include(s => s.Class)
                .Where(s => s.Status != ModelStatus.Deleted)
                .AsQueryable();

            if (classId.HasValue)
            {
                query = query.Where(s => s.ClassId == classId.Value);
            }

            return await query.ToListAsync();
        }

        public Task<Subject?> GetByIdAsync(long id, bool includeClass = false)
        {
            var query = _context.Subject.AsQueryable();

            if (includeClass)
                query = query.Include(s => s.Class);

            return query.FirstOrDefaultAsync(s => s.Id == id);
        }

        public Task<bool> NameExistsInClassAsync(string name, long? classId, long? excludeId = null)
        {
            var query = _context.Subject
                .Where(s => s.Name.ToLower() == name.ToLower());

            if (classId.HasValue)
                query = query.Where(s => s.ClassId == classId.Value);

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public async Task AddAsync(Subject subject)
        {
            _context.Subject.Add(subject);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Subject subject)
        {
            _context.Subject.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Subject>> GetPendingAsync()
        {
            return await _context.Subject
                .Include(s => s.Class)
                .Where(s => s.Status == ModelStatus.Pending || s.Status == ModelStatus.InActive)
                .OrderByDescending(s => s.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
