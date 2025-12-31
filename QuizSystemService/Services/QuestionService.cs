using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repo;
        private readonly IQuizRepository _quizRepo;

        public QuestionService(IQuestionRepository repo, IQuizRepository quizRepo)
        {
            _repo = repo;
            _quizRepo = quizRepo;
        }

        public Task<List<QuestionBank>> GetByQuizAsync(long quizId, string? subject = null) =>
            _repo.GetByQuizAsync(quizId, subject);

        public Task<QuestionBank?> GetByIdAsync(long id) => _repo.GetByIdAsync(id);

        public async Task<bool> CreateAsync(QuestionBank question, QuizSystemUser currentUser)
        {
            var quiz = await _quizRepo.GetByIdAsync(question.QuizId);
            if (quiz == null)
                throw new InvalidOperationException("Quiz not found.");

            if (string.IsNullOrWhiteSpace(question.QuestionText))
                throw new InvalidOperationException("Question text is required.");

            question.CreatedAt = DateTime.UtcNow;
            question.Status = ModelStatus.Active;
            question.CreatedBy = currentUser;

            await _repo.AddAsync(question);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, QuestionBank question, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(question.QuestionText))
                throw new InvalidOperationException("Question text is required.");

            existing.Name = question.Name;
            existing.Subject = question.Subject;
            existing.QuestionText = question.QuestionText;
            existing.Description = question.Description;
            existing.OptionA = question.OptionA;
            existing.OptionB = question.OptionB;
            existing.OptionC = question.OptionC;
            existing.OptionD = question.OptionD;
            existing.RightOption = question.RightOption;
            existing.Marks = question.Marks;
            existing.Status = question.Status;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.Status = ModelStatus.Deleted;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await _repo.UpdateAsync(existing);
            return true;
        }
    }
}
