using QuizSystemModel.BusinessRules;
using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;

        public SubjectService(ISubjectRepository repo)
        {
            _repo = repo;
        }

        public Task<List<Subject>> GetAllAsync(long? classId = null) =>
            _repo.GetAllAsync(classId);

        public Task<Subject?> GetByIdAsync(long id, bool includeClass = false) =>
            _repo.GetByIdAsync(id, includeClass);

        public async Task<bool> CreateAsync(Subject model, long? classId, QuizSystemUser currentUser)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Subject name is required.");

            if (await _repo.NameExistsInClassAsync(model.Name, classId))
                throw new InvalidOperationException("This subject already exists for the selected class.");

            model.CreatedAt = DateTime.UtcNow;
            model.Status = ModelStatus.Active;
            model.IsApproved = false;
            model.CreatedBy = currentUser;

            if (classId.HasValue)
                model.ClassId = classId.Value;

            await _repo.AddAsync(model);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, Subject model, long? classId)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return false;

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new InvalidOperationException("Subject name is required.");

            long? effectiveClassId = classId ?? existing.ClassId;

            if (await _repo.NameExistsInClassAsync(model.Name, effectiveClassId, id))
                throw new InvalidOperationException("This subject already exists for the selected class.");

            existing.Name = model.Name;
            existing.IsApproved = model.IsApproved;
            existing.Status = model.Status;
            existing.ModifiedAt = DateTime.UtcNow;
            existing.ClassId = effectiveClassId;

            await _repo.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(long id, QuizSystemUser currentUser)
        {
            var subject = await _repo.GetByIdAsync(id);
            if (subject == null)
                return false;

            subject.Status = ModelStatus.Deleted;
            subject.ModifiedAt = DateTime.UtcNow;
            subject.ModifiedBy = currentUser;

            await _repo.UpdateAsync(subject);
            return true;
        }

        public async Task<bool> ApproveAsync(long id, QuizSystemUser currentUser)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

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
            if (entity == null) return false;

            entity.IsApproved = false;
            entity.RejectedAt = DateTime.UtcNow;
            entity.RejectedBy = currentUser;
            entity.ApprovedAt = null;
            entity.ApprovedBy = null;

            await _repo.UpdateAsync(entity);
            return true;
        }

        public Task<List<Subject>> GetPendingAsync()
        {
            return _repo.GetPendingAsync();
        }
    }
}
