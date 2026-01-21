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
        private readonly IInstructorRepository _instructorRepo;

        public QuizService(IQuizRepository repo, IInstructorRepository instructorRepo)
        {
            _repo = repo;
            _instructorRepo = instructorRepo;
        }

        public async Task<List<Quiz>> GetAllAsync(QuizSystemUser currentUser = null, long? mediumId = null, long? classId = null, long? subjectId = null) 
        {
            if (currentUser != null)
            {
                var instructor = await _instructorRepo.GetByUserIdAsync(currentUser.Id);
                if (instructor != null)
                {
                    if (instructor.ClassId.HasValue)
                    {
                        
                        mediumId = instructor.EducationMediumId;
                        classId = instructor.ClassId;
                    }
                    else
                    {
                        return new List<Quiz>();
                    }
                }
            }
            return await _repo.GetAllAsync(mediumId, classId, subjectId); 
        }

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
                NegativeMarking = quiz.NegativeMarking,
                RequiredPassPercentage = quiz.RequiredPassPercentage
            };
        }

        public async Task<bool> CreateAsync(QuizViewModel model, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Quiz name is required.");

            var instructor = await _instructorRepo.GetByUserIdAsync(currentUser.Id);
            if (instructor != null)
            {
                if (!instructor.ClassId.HasValue)
                    throw new InvalidOperationException("You have not been assigned to a class yet. Please contact Admin.");

                
                if (model.ClassId != instructor.ClassId)
                    throw new InvalidOperationException("You can only create quizzes for your assigned class.");
                
                
                if (instructor.EducationMediumId.HasValue && model.EducationMediumId != instructor.EducationMediumId)
                     throw new InvalidOperationException("You can only create quizzes for your assigned education medium.");
                
                 
                 
            }

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
                NegativeMarking = model.NegativeMarking,
                RequiredPassPercentage = model.RequiredPassPercentage,
                CreatedAt = DateTime.UtcNow,
                Status = ModelStatus.Pending,
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

            var instructor = await _instructorRepo.GetByUserIdAsync(currentUser.Id);
            if (instructor != null)
            {
                 if (!instructor.ClassId.HasValue)
                    throw new InvalidOperationException("You have not been assigned to a class yet. Please contact Admin.");

                 if (quiz.ClassId != instructor.ClassId)
                    throw new InvalidOperationException("You cannot edit quizzes outside your assigned class.");
                 
                 if (model.ClassId != instructor.ClassId)
                     throw new InvalidOperationException("You cannot move a quiz to another class.");

                 if (quiz.IsApproved || quiz.Status == ModelStatus.Active)
                 {
                     quiz.IsApproved = false;
                     quiz.Status = ModelStatus.Pending;
                     quiz.ApprovedAt = null;
                     quiz.ApprovedBy = null;
                 }
            }

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
            quiz.NegativeMarking = model.NegativeMarking;
            quiz.RequiredPassPercentage = model.RequiredPassPercentage;
            quiz.ModifiedAt = DateTime.UtcNow;
            quiz.ModifiedBy = currentUser;

            await _repo.UpdateAsync(quiz);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var quiz = await _repo.GetByIdAsync(id);
            if (quiz == null) return false;

            var instructor = await _instructorRepo.GetByUserIdAsync(currentUser.Id);
            if (instructor != null)
            {
                if (!instructor.ClassId.HasValue)
                    throw new InvalidOperationException("You have not been assigned to a class yet. Please contact Admin.");

                if (quiz.ClassId != instructor.ClassId)
                    throw new InvalidOperationException("You cannot delete quizzes outside your assigned class.");
            }

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
            quiz.Status = ModelStatus.Active;
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
            quiz.Status = ModelStatus.InActive;
            quiz.RejectedAt = DateTime.UtcNow;
            quiz.RejectedBy = currentUser;
            quiz.ApprovedAt = null;
            quiz.ApprovedBy = null;

            await _repo.UpdateAsync(quiz);
            return true;
        }
    }
}
