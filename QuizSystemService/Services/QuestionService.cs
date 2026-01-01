using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
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

        public Task<QuestionBank?> GetEntityAsync(long id) => _repo.GetByIdAsync(id);

        public async Task<QuestionViewModel?> GetForEditAsync(long id)
        {
            var q = await _repo.GetByIdAsync(id);
            if (q == null) return null;

            return new QuestionViewModel
            {
                Id = q.Id,
                QuizId = q.QuizId,
                Subject = q.Subject,
                Name = q.Name,
                QuestionText = q.QuestionText,
                Description = q.Description,
                OptionA = q.OptionA,
                OptionB = q.OptionB,
                OptionC = q.OptionC,
                OptionD = q.OptionD,
                RightOption = q.RightOption,
                Marks = q.Marks
            };
        }

        public async Task<bool> CreateAsync(QuestionViewModel model, QuizSystemUser currentUser)
        {
            var quiz = await _quizRepo.GetByIdAsync(model.QuizId);
            if (quiz == null)
                throw new InvalidOperationException("Quiz not found.");

            if (string.IsNullOrWhiteSpace(model.QuestionText))
                throw new InvalidOperationException("Question text is required.");

            var q = new QuestionBank
            {
                QuizId = model.QuizId,
                Subject = model.Subject,
                Name = model.Name,
                QuestionText = model.QuestionText,
                Description = model.Description ?? "",
                OptionA = model.OptionA,
                OptionB = model.OptionB,
                OptionC = model.OptionC,
                OptionD = model.OptionD,
                RightOption = model.RightOption,
                Marks = model.Marks,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                CreatedBy = currentUser
            };

            await _repo.AddAsync(q);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, QuestionViewModel model, QuizSystemUser currentUser)
        {
            var q = await _repo.GetByIdAsync(id);
            if (q == null) return false;

            if (string.IsNullOrWhiteSpace(model.QuestionText))
                throw new InvalidOperationException("Question text is required.");

            q.Subject = model.Subject;
            q.Name = model.Name;
            q.QuestionText = model.QuestionText;
            q.Description = model.Description ?? "";
            q.OptionA = model.OptionA;
            q.OptionB = model.OptionB;
            q.OptionC = model.OptionC;
            q.OptionD = model.OptionD;
            q.RightOption = model.RightOption;
            q.Marks = model.Marks;
            q.ModifiedAt = DateTime.UtcNow;
            q.ModifiedBy = currentUser;
            q.Status = q.Status;

            await _repo.UpdateAsync(q);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var q = await _repo.GetByIdAsync(id);
            if (q == null) return false;

            q.Status = ModelStatus.Deleted;
            q.ModifiedAt = DateTime.UtcNow;
            q.ModifiedBy = currentUser;

            await _repo.UpdateAsync(q);
            return true;
        }
    }
}
