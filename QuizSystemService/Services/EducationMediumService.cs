using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class EducationMediumService : IEducationMediumService
    {
        private readonly IEducationMediumRepository _repo;

        public EducationMediumService(IEducationMediumRepository repo)
        {
            _repo = repo;
        }

        public Task<List<EducationMedium>> GetAllAsync() => _repo.GetAllAsync();

        public Task<EducationMedium?> GetByIdAsync(long id) => _repo.GetByIdAsync(id);

        public Task<List<Class>> GetClassesByMediumAsync(long mediumId) =>
            _repo.GetClassesByMediumAsync(mediumId);

        public async Task<bool> CreateAsync(EducationMedium model, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Name is required.");

            if (await _repo.NameExistsAsync(model.Name))
                throw new InvalidOperationException("This education medium already exists.");

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;

            await _repo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, EducationMedium model) // <- EXACT match
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Name is required.");

            if (await _repo.NameExistsAsync(model.Name, id))
                throw new InvalidOperationException("This education medium already exists.");

            existing.Name = model.Name;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.Status = model.Status;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _repo.GetByIdAsync(id);
            if (medium == null)
                return false;

            medium.Status = ModelStatus.Deleted;
            medium.ModifiedAt = DateTime.UtcNow;
            medium.ModifiedBy = currentUser;

            await _repo.UpdateAsync(medium);
            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _repo.GetByIdAsync(id);
            if (medium == null)
                return false;

            medium.IsApproved = true;
            medium.ApprovedAt = DateTime.UtcNow;
            medium.ApprovedBy = currentUser;
            medium.RejectedAt = null;
            medium.RejectedBy = null;

            await _repo.UpdateAsync(medium);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var medium = await _repo.GetByIdAsync(id);
            if (medium == null)
                return false;

            medium.IsApproved = false;
            medium.RejectedAt = DateTime.UtcNow;
            medium.RejectedBy = currentUser;
            medium.ApprovedAt = null;
            medium.ApprovedBy = null;

            await _repo.UpdateAsync(medium);
            return true;
        }
    }
}
