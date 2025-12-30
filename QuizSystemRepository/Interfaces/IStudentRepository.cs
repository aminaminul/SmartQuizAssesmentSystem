using QuizSystemModel.Models;

namespace QuizSystemModel.Interfaces
{
    public interface IStudentRepository
    {
        Task<List<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(long id, bool includeUser = false);
        Task<bool> EmailExistsAsync(string email, long? excludeId = null);
        Task<bool> PhoneExistsAsync(string phone, long? excludeId = null);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task SaveChangesAsync();

        Task<List<EducationMedium>> GetEducationMediumsAsync();
        Task<List<Class>> GetClassesAsync();
    }
}
