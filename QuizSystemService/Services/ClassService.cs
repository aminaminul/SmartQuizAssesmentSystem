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

        public async Task<List<Class>> GetAllAsync(long? educationMediumId = null)
        {
            return await _repo.GetAllAsync(educationMediumId);
        }

        public async Task<Class?> GetByIdAsync(long id, bool includeMedium = false)
        {
            return await _repo.GetByIdAsync(id, includeMedium);
        }

        public async Task<List<EducationMedium>> GetEducationMediumsAsync()
        {
            return await _repo.GetEducationMediumsAsync();
        }


        public async Task<bool> CreateAsync(Class model, long educationMediumId, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Class name is required.");

            // check Medium
            var mediums = await _repo.GetEducationMediumsAsync();
            var medium = mediums.FirstOrDefault(m => m.Id == educationMediumId);
            if (medium == null)
                throw new InvalidOperationException("Selected education medium does not exist.");

            // Duplicate Class Name Check within same Medium
            if (await _repo.NameExistsInMediumAsync(model.Name, educationMediumId))
                throw new InvalidOperationException("This class already exists for the selected education medium.");

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;
            model.EducationMediumId = educationMediumId;

            await _repo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Class model)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Class name is required.");

            // duplicate name check within same medium
            if (existing.EducationMediumId.HasValue &&
                await _repo.NameExistsInMediumAsync(model.Name, existing.EducationMediumId.Value, id))
                throw new InvalidOperationException("This class already exists for the selected education medium.");

            existing.Name = model.Name;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var cls = await _repo.GetByIdAsync(id);
            if (cls == null)
                return false;

            cls.Status = ModelStatus.Deleted;
            cls.ModifiedAt = DateTime.UtcNow;
            cls.ModifiedBy = currentUser;

            await _repo.UpdateAsync(cls);
            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var cls = await _repo.GetByIdAsync(id);
            if (cls == null)
                return false;

            cls.IsApproved = true;
            cls.ApprovedAt = DateTime.UtcNow;
            cls.ApprovedBy = currentUser;
            cls.RejectedAt = null;
            cls.RejectedBy = null;

            await _repo.UpdateAsync(cls);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var cls = await _repo.GetByIdAsync(id);
            if (cls == null)
                return false;

            cls.IsApproved = false;
            cls.RejectedAt = DateTime.UtcNow;
            cls.RejectedBy = currentUser;
            cls.ApprovedAt = null;
            cls.ApprovedBy = null;

            await _repo.UpdateAsync(cls);
            return true;
        }
    }
}
