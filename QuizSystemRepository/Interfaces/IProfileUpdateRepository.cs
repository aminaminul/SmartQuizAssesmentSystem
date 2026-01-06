using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IProfileUpdateRepository
    {
        Task<ProfileUpdateRequest?> GetLastApprovedAsync(long userId);
        Task<ProfileUpdateRequest?> GetLastPendingAsync(long userId);

        Task AddAsync(ProfileUpdateRequest request);
        Task UpdateAsync(ProfileUpdateRequest request);

        Task<List<ProfileUpdateRequest>> GetPendingRequestsAsync();
        Task<ProfileUpdateRequest?> GetByIdAsync(long id);
    }
}
