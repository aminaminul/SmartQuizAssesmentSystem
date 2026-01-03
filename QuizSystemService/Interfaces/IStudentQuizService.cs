using QuizSystemModel.Models;

namespace QuizSystemService.Interfaces
{
    public interface IStudentQuizService
    {
        Task<List<Quiz>> GetAvailableQuizzesAsync(long studentUserId);
        Task<QuizAttempt> StartAttemptAsync(long quizId, long studentUserId);
        Task<QuizAttempt?> GetAttemptWithQuestionsAsync(long attemptId, long studentUserId);
        Task AutosaveAnswerAsync(long attemptId, long questionId, string? selectedOption);
        Task SubmitAttemptAsync(long attemptId, long studentUserId);
    }
}
