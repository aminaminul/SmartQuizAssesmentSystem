using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class EducationMediumRepository : IEducationMediumRepository
    {
        private readonly AppDbContext _context;

        public EducationMediumRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<EducationMedium>> GetAllAsync()
        {
            return _context.EducationMedium
                .Where(m => m.Status != ModelStatus.Deleted)
                .ToListAsync();
        }

        public Task<EducationMedium?> GetByIdAsync(long id)
        {
            return _context.EducationMedium
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public Task<bool> NameExistsAsync(string name, long? excludeId = null)
        {
            var query = _context.EducationMedium
                .Where(m => m.Name.ToLower() == name.ToLower());

            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);

            return query.AnyAsync();
        }

        public async Task AddAsync(EducationMedium medium)
        {
            _context.EducationMedium.Add(medium);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EducationMedium medium)
        {
            _context.EducationMedium.Update(medium);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EducationMedium medium)
        {
            _context.EducationMedium.Remove(medium);
            await _context.SaveChangesAsync();
        }

        public Task<List<EducationMedium>> GetPendingAsync()
        {
            return _context.EducationMedium
                .Where(m => m.Status == ModelStatus.Pending || m.Status == ModelStatus.InActive)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
