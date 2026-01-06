using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;

        public ClassService(IClassRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Class>> GetAllAsync(long? educationMediumId = null) =>
            _repo.GetAllAsync(educationMediumId);

        public Task<Class?> GetByIdAsync(long id, bool includeMedium = false) =>
            _repo.GetByIdAsync(id, includeMedium);

        public Task<List<Class>> GetPendingAsync() =>
            _repo.GetPendingAsync();

        public async Task<bool> CreateAsync(ClassNameEnum className, long educationMediumId, QuizSystemUser currentUser)
        {
            if (await _repo.NameExistsInMediumAsync(className, educationMediumId))
                throw new InvalidOperationException("This class already exists for the selected education medium.");

            var cls = new Class
            {
                ClassName = className,
                EducationMediumId = educationMediumId,
                Status = ModelStatus.Active,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(cls);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, ClassNameEnum className, long educationMediumId, ModelStatus status)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            if (await _repo.NameExistsInMediumAsync(className, educationMediumId, id))
                throw new InvalidOperationException("This class already exists for the selected education medium.");

            existing.ClassName = className;
            existing.EducationMediumId = educationMediumId;
            existing.Status = status;
            existing.ModifiedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Status = ModelStatus.Deleted;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsApproved = true;
            existing.ApprovedAt = DateTime.UtcNow;
            existing.Status = ModelStatus.Active;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsApproved = false;
            existing.RejectedAt = DateTime.UtcNow;
            existing.Status = ModelStatus.InActive;

            await _repo.UpdateAsync(existing);
            return true;
        }
    }
}
