using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class EducationMediumService : IEducationMediumService
    {
        private readonly IEducationMediumRepository _repo;
        private readonly IClassRepository _classRepo;

        public EducationMediumService(
            IEducationMediumRepository repo,
            IClassRepository classRepo)
        {
            _repo = repo;
            _classRepo = classRepo;
        }

        public Task<List<EducationMedium>> GetAllAsync() =>
            _repo.GetAllAsync();

        public Task<EducationMedium?> GetByIdAsync(long id) =>
            _repo.GetByIdAsync(id);

        public Task<List<Class>> GetClassesByMediumAsync(long mediumId) =>
            _classRepo.GetByMediumAsync(mediumId);

        public async Task<bool> CreateAsync(EducationMedium model, QuizSystemUser currentUser)
        {
            if (await _repo.NameExistsAsync(model.Name))
                throw new InvalidOperationException("This medium already exists.");

            await _repo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, EducationMedium model)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            if (await _repo.NameExistsAsync(model.Name, id))
                throw new InvalidOperationException("This medium already exists.");

            existing.Name = model.Name;
            existing.Status = model.Status;
            existing.IsApproved = model.IsApproved;

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
            existing.Status = ModelStatus.Active;
            existing.ApprovedAt = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            existing.IsApproved = false;
            existing.Status = ModelStatus.InActive;
            existing.RejectedAt = DateTime.UtcNow;
            existing.ModifiedBy = currentUser;

            await _repo.UpdateAsync(existing);
            return true;
        }
    }
}
