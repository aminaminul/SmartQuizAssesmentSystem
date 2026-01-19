using Microsoft.EntityFrameworkCore;
using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
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

        public Task<Quiz?> GetEntityAsync(long id, bool includeQuestions = false) =>
            _repo.GetByIdAsync(id, includeQuestions);

        public async Task<QuizViewModel?> GetForEditAsync(long id)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return null;

            return new QuizViewModel
            {
                Id = quiz.Id,
                Name = quiz.Name,
                SubjectId = quiz.SubjectId,
                ClassId = quiz.ClassId,
                EducationMediumId = quiz.EducationMediumId,
                Description = quiz.Description,
                StartAt = quiz.StartAt,
                EndAt = quiz.EndAt,
                DurationMinutes = quiz.Duration.HasValue ? (int?)quiz.Duration.Value.TotalMinutes : null,
                TotalMarks = quiz.TotalMarks,
                RequiredPassPercentage = quiz.RequiredPassPercentage
            };
        }

        public async Task<bool> CreateAsync(QuizViewModel model, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Quiz name is required.");

            var quiz = new Quiz
            {
                Name = model.Name,
                SubjectId = model.SubjectId,
                ClassId = model.ClassId,
                EducationMediumId = model.EducationMediumId,
                Description = model.Description,
                StartAt = model.StartAt,
                EndAt = model.EndAt,
                Duration = model.DurationMinutes.HasValue
                    ? TimeSpan.FromMinutes(model.DurationMinutes.Value)
                    : null,
                TotalMarks = model.TotalMarks,
                RequiredPassPercentage = model.RequiredPassPercentage,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Active,
                IsApproved = false,
                CreatedBy = currentUser
            };

            await _repo.AddAsync(quiz);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, QuizViewModel model, QuizSystemUser currentUser)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Quiz name is required.");

            quiz.Name = model.Name;
            quiz.SubjectId = model.SubjectId;
            quiz.ClassId = model.ClassId;
            quiz.EducationMediumId = model.EducationMediumId;
            quiz.Description = model.Description;
            quiz.StartAt = model.StartAt;
            quiz.EndAt = model.EndAt;
            quiz.Duration = model.DurationMinutes.HasValue
                ? TimeSpan.FromMinutes(model.DurationMinutes.Value)
                : null;
            quiz.TotalMarks = model.TotalMarks;
            quiz.RequiredPassPercentage = model.RequiredPassPercentage;
            quiz.Status = quiz.Status; // if editable, set from model
            quiz.ModifiedAt = DateTime.UtcNow;
            quiz.ModifiedBy = currentUser;

            await _repo.UpdateAsync(quiz);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return false;

            quiz.Status = ModelStatus.Deleted;
            quiz.ModifiedAt = DateTime.UtcNow;
            quiz.ModifiedBy = currentUser;

            await _repo.UpdateAsync(quiz);
            return true;
        }

        public Task<List<Quiz>> GetPendingAsync()
        {
            return _repo.GetPendingAsync();
        }
        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return false;

            quiz.IsApproved = true;
            quiz.ApprovedAt = DateTime.UtcNow;
            quiz.ApprovedBy = currentUser;
            quiz.RejectedAt = null;
            quiz.RejectedBy = null;

            await _repo.UpdateAsync(quiz);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return false;

            quiz.IsApproved = false;
            quiz.RejectedAt = DateTime.UtcNow;
            quiz.RejectedBy = currentUser;
            quiz.ApprovedAt = null;
            quiz.ApprovedBy = null;

            await _repo.UpdateAsync(quiz);
            return true;
        }
    }
}
