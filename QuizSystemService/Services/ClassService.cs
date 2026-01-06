using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repo;
        private readonly IEducationMediumRepository _mediumRepo;

        public ClassService(IClassRepository repo, IEducationMediumRepository mediumRepo)
        {
            _repo = repo;
            _mediumRepo = mediumRepo;
        }

        public async Task<List<Class>> GetAllAsync(EducationMediums? educationMediumId = null)
        {
            var classes = await _repo.GetAllAsync(educationMediumId);
            return classes;
        }

        public async Task<Class?> GetByIdAsync(long id, bool includeMedium = false)
        {
            var cls = await _repo.GetByIdAsync(id, includeMedium);
            return cls;
        }


        public async Task<bool> CreateAsync(Class model,EducationMediums educationMedium, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Class Name Is Required.");

            if (await _repo.NameExistsInMediumAsync(model.Name, educationMedium))
                throw new InvalidOperationException(
                    "This Class Already Exists For The Selected Education Medium.");

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;
            model.EducationMediumId = educationMedium;

            await _repo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Class model)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Class Name Is Required.");

            if (existing.EducationMediumId.HasValue &&
                await _repo.NameExistsInMediumAsync(model.Name, existing.EducationMediumId.Value, id))
                throw new InvalidOperationException("This Class Already Exists Ror The Selected Education Medium.");

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
        public Task<List<Class>> GetPendingAsync()
        {
            return _repo.GetPendingAsync();
        }
        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return false;

            entity.IsApproved = true;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.ApprovedBy = currentUser;
            entity.RejectedAt = null;
            entity.RejectedBy = null;

            await _repo.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> RejectAsync(long id, QuizSystemUser currentUser)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return false;

            entity.IsApproved = false;
            entity.RejectedAt = DateTime.UtcNow;
            entity.RejectedBy = currentUser;
            entity.ApprovedAt = null;
            entity.ApprovedBy = null;

            await _repo.UpdateAsync(entity);
            return true;
        }
    }
}
