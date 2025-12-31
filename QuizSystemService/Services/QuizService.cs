using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _repo;

        public QuizService(IQuizRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Quiz>> GetAllAsync() => _repo.GetAllAsync();

        public Task<Quiz?> GetByIdAsync(long id, bool includeQuestions = false) =>
            _repo.GetByIdAsync(id, includeQuestions);

        public async Task<bool> CreateAsync(Quiz quiz, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(quiz.Name))
                throw new InvalidOperationException("Quiz name is required.");

            quiz.CreatedAt = DateTime.UtcNow;
            quiz.Status = ModelStatus.Active;
            quiz.IsApproved = false;
            quiz.CreatedBy = currentUser;

            await _repo.AddAsync(quiz);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Quiz quiz, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(quiz.Name))
                throw new InvalidOperationException("Quiz name is required.");

            existing.Name = quiz.Name;
            existing.Description = quiz.Description;
            existing.Subject = quiz.Subject;
            existing.StartAt = quiz.StartAt;
            existing.EndAt = quiz.EndAt;
            existing.Duration = quiz.Duration;
            existing.TotalMarks = quiz.TotalMarks;
            existing.RequiredPassPercentage = quiz.RequiredPassPercentage;
            existing.Status = quiz.Status;
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

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.IsApproved = true;
            existing.ApprovedAt = DateTime.UtcNow;
            existing.ApprovedBy = currentUser;
            existing.RejectedAt = null;
            existing.RejectedBy = null;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.IsApproved = false;
            existing.RejectedAt = DateTime.UtcNow;
            existing.RejectedBy = currentUser;
            existing.ApprovedAt = null;
            existing.ApprovedBy = null;

            await _repo.UpdateAsync(existing);
            return true;
        }
    }
}
