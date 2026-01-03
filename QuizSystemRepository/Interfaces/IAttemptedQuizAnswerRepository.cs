namespace QuizSystemRepository.Interfaces
{
    public interface IAttemptedQuizAnswerRepository
    {
        Task<List<AttemptedQuizAnswer>> GetByAttemptIdAsync(long attemptId);
        Task<AttemptedQuizAnswer?> GetByAttemptAndQuestionAsync(long attemptId, long questionId);
        Task AddAsync(AttemptedQuizAnswer answer);
        Task UpdateAsync(AttemptedQuizAnswer answer);
        Task SaveChangesAsync();
    }
}
