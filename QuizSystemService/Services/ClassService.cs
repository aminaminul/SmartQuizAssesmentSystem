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

        public Task<List<Class>> GetPendingAsync() => _repo.GetPendingAsync();

        public async Task<bool> CreateAsync(long classId, long educationMediumId, QuizSystemUser currentUser)
        {
            var className = GetClassNameById(classId);

            if (await _repo.NameExistsInMediumAsync(className, educationMediumId))
                throw new InvalidOperationException("This Class Already Exists For The Selected Education Medium.");

            var cls = new Class
            {
                Name = className,
                EducationMediumId = educationMediumId,
                Status = ModelStatus.Pending,
                IsApproved = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUser,
                ModifiedAt = DateTime.UtcNow,
                ModifiedBy = currentUser
            };

            await _repo.AddAsync(cls);
            return true;
        }

        public async Task<bool> UpdateAsync(long id, long classId, long educationMediumId, ModelStatus status, QuizSystemUser currentUser)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return false;

            var className = GetClassNameById(classId);

            if (await _repo.NameExistsInMediumAsync(className, educationMediumId, id))
                throw new InvalidOperationException("This Class Already Exists For The Selected Education Medium.");

            existing.Name = className;
            existing.EducationMediumId = educationMediumId;
            existing.Status = status;
            existing.ModifiedBy = currentUser;
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
            existing.ApprovedBy = currentUser;
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
            existing.RejectedBy = currentUser;
            existing.Status = ModelStatus.InActive;

            await _repo.UpdateAsync(existing);
            return true;
        }

        private static string GetClassNameById(long classId)
        {
            if (Enum.IsDefined(typeof(ClassNameEnum), (int)classId))
            {
                return ((ClassNameEnum)classId).ToString();
            }
            throw new ArgumentException($"Invalid Class ID: {classId}");
        }
    }
}
