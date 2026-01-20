using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IInstructorRepository
    {
        Task<List<Instructor>> GetAllAsync();
        Task<Instructor?> GetByIdAsync(long id);
        Task<bool> EmailExistsAsync(string email, long? excludeId = null);
        Task<bool> PhoneExistsAsync(string phone, long? excludeId = null);
        Task AddAsync(Instructor instructor);
        Task UpdateAsync(Instructor instructor);
        Task SaveChangesAsync();
        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task<List<Class>> GetClassesAsync();
        Task<Instructor?> GetByUserIdAsync(long userId);
        Task<List<Instructor>> GetPendingAsync();
    }
}
