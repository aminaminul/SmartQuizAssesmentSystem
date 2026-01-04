using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private readonly AppDbContext _context;

        public ClassRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Class>> GetAllAsync(long? educationMediumId = null)
        {
            var query = _context.Class
                .Include(c => c.EducationMedium)
                .Where(c => c.Status != ModelStatus.Deleted)
                .AsQueryable();

            if (educationMediumId.HasValue)
            {
                query = query.Where(c => c.EducationMediumId == educationMediumId.Value);
            }

            return await query.ToListAsync();
        }

        public Task<Class?> GetByIdAsync(long id, bool includeMedium = false)
        {
            var query = _context.Class.AsQueryable();

            if (includeMedium)
                query = query.Include(c => c.EducationMedium);

            return query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public Task<bool> NameExistsInMediumAsync(string name, long educationMediumId, long? excludeId = null)
        {
            var query = _context.Class
                .Where(c => c.Name.ToLower() == name.ToLower() &&
                            c.EducationMediumId == educationMediumId);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public Task<List<Class>> GetByMediumAsync(long mediumId)
        {
            return _context.Class
                .Where(c => c.EducationMediumId == mediumId &&
                            c.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

        public async Task AddAsync(Class cls)
        {
            _context.Class.Add(cls);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Class cls)
        {
            _context.Class.Update(cls);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Class>> GetPendingAsync()
        {
            return await _context.Class
                .Where(c => !c.IsApproved
                            && c.Status == ModelStatus.Active)
                .OrderByDescending(c => c.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
