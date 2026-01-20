using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Data;

namespace QuizSystemRepository.Repositories
{
    public class ProfileUpdateRepository : IProfileUpdateRepository
    {
        private readonly AppDbContext _context;

        public ProfileUpdateRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<ProfileUpdateRequest?> GetLastApprovedAsync(long userId)
        {
            return _context.ProfileUpdateRequests
                .Where(r => r.UserId == userId && r.Status == ProfileUpdateStatus.Approved)
                .OrderByDescending(r => r.ApprovedAt)
                .FirstOrDefaultAsync();
        }

        public Task<ProfileUpdateRequest?> GetLastPendingAsync(long userId)
        {
            return _context.ProfileUpdateRequests
                .Where(r => r.UserId == userId && r.Status == ProfileUpdateStatus.Pending)
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(ProfileUpdateRequest request)
        {
            await _context.ProfileUpdateRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProfileUpdateRequest request)
        {
            _context.ProfileUpdateRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public Task<List<ProfileUpdateRequest>> GetPendingRequestsAsync()
        {
            return _context.ProfileUpdateRequests
                .Where(r => r.Status == ProfileUpdateStatus.Pending || r.Status == ProfileUpdateStatus.Rejected)
                .OrderBy(r => r.RequestedAt)
                .ToListAsync();
        }

        public Task<ProfileUpdateRequest?> GetByIdAsync(long id)
        {
            return _context.ProfileUpdateRequests
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}
