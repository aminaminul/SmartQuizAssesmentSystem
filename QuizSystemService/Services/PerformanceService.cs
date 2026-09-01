using QuizSystemModel.Models;
using QuizSystemModel.ViewModels;
using QuizSystemModel.Interfaces;
using QuizSystemRepository.Interfaces;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly IQuizAttemptRepository _attemptRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IClassRepository _classRepo;
        private readonly IEducationMediumRepository _mediumRepo;

        public PerformanceService(
            IQuizAttemptRepository attemptRepo,
            IStudentRepository studentRepo,
            IClassRepository classRepo,
            IEducationMediumRepository mediumRepo)
        {
            _attemptRepo = attemptRepo;
            _studentRepo = studentRepo;
            _classRepo = classRepo;
            _mediumRepo = mediumRepo;
        }

        public async Task<StudentPerformanceViewModel> GetStudentPerformanceAsync(long? studentId = null, long? classId = null, long? mediumId = null)
        {
            var attempts = await _attemptRepo.GetAllAttemptsAsync();
            var students = await _studentRepo.GetAllAsync(classId, mediumId);

            if (studentId.HasValue)
            {
                students = students.Where(s => s.Id == studentId.Value).ToList();
            }

            var result = new StudentPerformanceViewModel
            {
                SelectedClassId = classId,
                SelectedMediumId = mediumId
            };

            foreach (var student in students)
            {
                var studentAttempts = attempts.Where(a => a.StudentUserId == student.UserId).ToList();
                var item = new StudentPerformanceItem
                {
                    StudentId = student.Id,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    Email = student.Email,
                    ClassName = student.Class?.Name,
                    MediumName = student.EducationMedium?.Name,
                    TotalAttempts = studentAttempts.Count,
                    AverageScore = studentAttempts.Any() ? Math.Round(studentAttempts.Average(a => a.TotalScore), 2) : 0,
                    QuizzesPassed = studentAttempts.Count(a => a.IsPassed),
                    QuizzesFailed = studentAttempts.Count(a => !a.IsPassed)
                };

                if (item.TotalAttempts > 0)
                {
                    item.PassRate = Math.Round((decimal)item.QuizzesPassed / item.TotalAttempts * 100, 2);
                }

                result.Students.Add(item);
            }

            return result;
        }

        public async Task<ClassPerformanceViewModel> GetClassPerformanceAsync(long? classId = null, long? mediumId = null)
        {
            var attempts = await _attemptRepo.GetAllAttemptsAsync();
            var classes = await _classRepo.GetAllAsync(mediumId);
            
            if (classId.HasValue)
            {
                classes = classes.Where(c => c.Id == classId.Value).ToList();
            }

            var students = await _studentRepo.GetAllAsync(null, mediumId);
            
            var result = new ClassPerformanceViewModel
            {
                SelectedMediumId = mediumId
            };

            foreach (var cls in classes)
            {
                var classStudents = students.Where(s => s.ClassId == cls.Id).ToList();
                var studentUserIds = classStudents.Select(s => s.UserId).ToHashSet();
                
                var classAttempts = attempts.Where(a => studentUserIds.Contains(a.StudentUserId)).ToList();

                var item = new ClassPerformanceItem
                {
                    ClassId = cls.Id,
                    ClassName = cls.Name,
                    MediumName = cls.EducationMedium?.Name,
                    TotalStudents = classStudents.Count,
                    TotalAttempts = classAttempts.Count,
                    AverageScore = classAttempts.Any() ? Math.Round(classAttempts.Average(a => a.TotalScore), 2) : 0
                };

                if (item.TotalAttempts > 0)
                {
                    var passed = classAttempts.Count(a => a.IsPassed);
                    item.PassRate = Math.Round((decimal)passed / item.TotalAttempts * 100, 2);
                }

                result.Classes.Add(item);
            }

            return result;
        }

        public async Task<MediumPerformanceViewModel> GetMediumPerformanceAsync(long? mediumId = null)
        {
            var attempts = await _attemptRepo.GetAllAttemptsAsync();
            var mediums = await _mediumRepo.GetAllAsync();

            if (mediumId.HasValue)
            {
                mediums = mediums.Where(m => m.Id == mediumId.Value).ToList();
            }

            var students = await _studentRepo.GetAllAsync();
            var classes = await _classRepo.GetAllAsync();

            var result = new MediumPerformanceViewModel();

            foreach (var medium in mediums)
            {
                var mediumStudents = students.Where(s => s.EducationMediumId == medium.Id).ToList();
                var studentUserIds = mediumStudents.Select(s => s.UserId).ToHashSet();
                var mediumAttempts = attempts.Where(a => studentUserIds.Contains(a.StudentUserId)).ToList();
                
                var item = new MediumPerformanceItem
                {
                    MediumId = medium.Id,
                    MediumName = medium.Name,
                    TotalClasses = classes.Count(c => c.EducationMediumId == medium.Id),
                    TotalStudents = mediumStudents.Count,
                    TotalAttempts = mediumAttempts.Count,
                    AverageScore = mediumAttempts.Any() ? Math.Round(mediumAttempts.Average(a => a.TotalScore), 2) : 0
                };

                if (item.TotalAttempts > 0)
                {
                    var passed = mediumAttempts.Count(a => a.IsPassed);
                    item.PassRate = Math.Round((decimal)passed / item.TotalAttempts * 100, 2);
                }

                result.Mediums.Add(item);
            }

            return result;
        }

        public async Task<MyPerformanceViewModel> GetMyPerformanceAsync(long studentUserId)
        {
            var attempts = await _attemptRepo.GetAllAttemptsAsync();
            var myAttempts = attempts.Where(a => a.StudentUserId == studentUserId).ToList();

            var student = await _studentRepo.GetByUserIdAsync(studentUserId);
            if (student == null) throw new Exception("Student not found");

            var result = new MyPerformanceViewModel
            {
                StudentName = $"{student.FirstName} {student.LastName}",
                Email = student.Email,
                ClassName = student.Class?.Name,
                MediumName = student.EducationMedium?.Name,
                TotalAttempts = myAttempts.Count,
                AverageScore = myAttempts.Any() ? Math.Round(myAttempts.Average(a => a.TotalScore), 2) : 0,
                QuizzesPassed = myAttempts.Count(a => a.IsPassed),
                QuizzesFailed = myAttempts.Count(a => !a.IsPassed)
            };

            if (result.TotalAttempts > 0)
            {
                result.PassRate = Math.Round((decimal)result.QuizzesPassed / result.TotalAttempts * 100, 2);
            }

            result.RecentAttempts = myAttempts
                .OrderByDescending(a => a.StartAt)
                .Take(10)
                .Select(a => new QuizAttemptSummary
                {
                    AttemptId = a.Id,
                    QuizName = a.Quiz.Name,
                    SubjectName = a.Quiz.Subject?.Name,
                    AttemptDate = a.StartAt,
                    Score = a.TotalScore,
                    TotalMarks = a.Quiz.TotalMarks,
                    Passed = a.IsPassed
                })
                .ToList();

            return result;
        }
    }
}
