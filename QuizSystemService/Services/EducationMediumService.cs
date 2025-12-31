using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class EducationMediumService : IEducationMediumService
    {
        private readonly IEducationMediumRepository _mediumRepo;
        private readonly IClassRepository _classRepo;

        public EducationMediumService(
            IEducationMediumRepository mediumRepo,
            IClassRepository classRepo)
        {
            _mediumRepo = mediumRepo;
            _classRepo = classRepo;
        }

        public Task<List<EducationMedium>> GetAllAsync() => _mediumRepo.GetAllAsync();

        public Task<EducationMedium?> GetByIdAsync(long id) => _mediumRepo.GetByIdAsync(id);

        public Task<List<Class>> GetClassesByMediumAsync(long mediumId) =>
            _classRepo.GetByMediumAsync(mediumId);

        public async Task<bool> CreateAsync(EducationMedium model, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Name is required.");

            if (await _mediumRepo.NameExistsAsync(model.Name))
                throw new InvalidOperationException("This education medium already exists.");

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;

            await _mediumRepo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, EducationMedium model)
        {
            var existing = await _mediumRepo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Name is required.");

            if (await _mediumRepo.NameExistsAsync(model.Name, id))
                throw new InvalidOperationException("This education medium already exists.");

            existing.Name = model.Name;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.Status = model.Status;

            await _mediumRepo.UpdateAsync(existing);
            return true;
        }

        // Medium + related classes soft delete
        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _mediumRepo.GetByIdAsync(id);
            if (medium == null)
                return false;

            // 1) medium
            medium.Status = ModelStatus.Deleted;
            medium.ModifiedAt = DateTime.UtcNow;
            medium.ModifiedBy = currentUser;
            await _mediumRepo.UpdateAsync(medium);

            // 2) related classes
            var classes = await _classRepo.GetByMediumAsync(id);
            foreach (var cls in classes)
            {
                cls.Status = ModelStatus.Deleted;
                cls.ModifiedAt = DateTime.UtcNow;
                cls.ModifiedBy = currentUser;
                await _classRepo.UpdateAsync(cls);
            }

            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _mediumRepo.GetByIdAsync(id);
            if (medium == null)
                return false;

            medium.IsApproved = true;
            medium.ApprovedAt = DateTime.UtcNow;
            medium.ApprovedBy = currentUser;
            medium.RejectedAt = null;
            medium.RejectedBy = null;

            await _mediumRepo.UpdateAsync(medium);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _mediumRepo.GetByIdAsync(id);
            if (medium == null)
                return false;

            medium.IsApproved = false;
            medium.RejectedAt = DateTime.UtcNow;
            medium.RejectedBy = currentUser;
            medium.ApprovedAt = null;
            medium.ApprovedBy = null;

            await _mediumRepo.UpdateAsync(medium);
            return true;
        }
    }
}
