namespace QuizSystemRepository.Interfaces
{
    public interface IQuizAttemptRepository
    {
        Task<QuizAttempt?> GetByIdAsync(long id);
        Task AddAsync(QuizAttempt attempt);
        Task UpdateAsync(QuizAttempt attempt);
        Task SaveChangesAsync();
    }
}