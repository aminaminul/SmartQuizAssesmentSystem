using QuizSystemModel.Interfaces;
using QuizSystemModel.Models;
using QuizSystemRepository.Interfaces;
using QuizSystemService.Interfaces;

namespace QuizSystemService.Services
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly IQuizRepository _quizRepository;
        private readonly IQuizAttemptRepository _attemptRepository;
        private readonly IAttemptedQuizAnswerRepository _answerRepository;
        private readonly IAccountRepository _accountRepository;

        public StudentQuizService(
            IQuizRepository quizRepository,
            IQuizAttemptRepository attemptRepository,
            IAttemptedQuizAnswerRepository answerRepository,
            IAccountRepository accountRepository)
        {
            _quizRepository = quizRepository;
            _attemptRepository = attemptRepository;
            _answerRepository = answerRepository;
            _accountRepository = accountRepository;
        }

        public async Task<List<Quiz>> GetAvailableQuizzesAsync(long studentUserId)
        {
            var now = DateTime.UtcNow;
            return await _quizRepository.GetAvailableForStudentAsync(studentUserId, now);
        }

        public async Task<QuizAttempt> StartAttemptAsync(long quizId, long studentUserId)
        {
            var quiz = await _quizRepository.GetByIdWithQuestionsAsync(quizId);
            if (quiz == null)
                throw new Exception("Quiz Not Found");

            
            var student = await _accountRepository.GetStudentByUserIdAsync(studentUserId);
            if (student == null || student.EducationMediumId != quiz.EducationMediumId || student.ClassId != quiz.ClassId)
                throw new Exception("You are not authorized to attempt this quiz.");

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                StudentUserId = studentUserId,
                StartAt = DateTime.UtcNow,
                LastSavedAt = DateTime.UtcNow,
                IsSubmitted = false
            };

            await _attemptRepository.AddAsync(attempt);
            await _attemptRepository.SaveChangesAsync();

            foreach (var q in quiz.Questions)
            {
                var ans = new AttemptedQuizAnswer
                {
                    QuizAttemptId = attempt.Id,
                    QuestionBankId = q.Id,
                    SelectedOption = null,
                    Score = 0
                };
                await _answerRepository.AddAsync(ans);
            }
            await _answerRepository.SaveChangesAsync();

            return attempt;
        }

        public async Task<QuizAttempt?> GetAttemptWithQuestionsAsync(long attemptId, long studentUserId)
        {
            var attempt = await _attemptRepository.GetByIdAsync(attemptId);
            if (attempt == null || attempt.StudentUserId != studentUserId)
                return null;

            return attempt;
        }

        public async Task AutosaveAnswerAsync(long attemptId, long questionId, string? selectedOption)
        {
            var answer = await _answerRepository.GetByAttemptAndQuestionAsync(attemptId, questionId);
            if (answer == null) return;

            answer.SelectedOption = selectedOption;
            await _answerRepository.UpdateAsync(answer);

            var attempt = await _attemptRepository.GetByIdAsync(attemptId);
            if (attempt != null && !attempt.IsSubmitted)
            {
                attempt.LastSavedAt = DateTime.UtcNow;
                await _attemptRepository.UpdateAsync(attempt);
            }

            await _answerRepository.SaveChangesAsync();
            await _attemptRepository.SaveChangesAsync();
        }

        public async Task SubmitAttemptAsync(long attemptId, long studentUserId)
        {
            var attempt = await _attemptRepository.GetByIdAsync(attemptId);
            if (attempt == null || attempt.StudentUserId != studentUserId)
                throw new Exception("Attempt not found");

            if (attempt.IsSubmitted) return;

            var quiz = attempt.Quiz;
            var answers = await _answerRepository.GetByAttemptIdAsync(attemptId);

            decimal total = 0;
            foreach (var ans in answers)
            {
                var q = ans.QuestionBank;
                // Normalize strings for comparison
                var selected = ans.SelectedOption?.Trim();
                var correct = q.RightOption?.Trim();

                if (!string.IsNullOrEmpty(selected) &&
                    string.Equals(selected, correct, StringComparison.OrdinalIgnoreCase))
                {
                    ans.Score = q.Marks;
                    total += q.Marks;
                }
                else
                {
                    ans.Score = 0;
                }

                await _answerRepository.UpdateAsync(ans);
            }

            attempt.TotalScore = total;
            attempt.EndAt = DateTime.UtcNow;
            attempt.IsSubmitted = true;

            // Ensure TotalMarks is not zero to avoid division by zero
            var totalMarks = quiz.TotalMarks > 0 ? quiz.TotalMarks : 1;
            var passMarks = totalMarks * quiz.RequiredPassPercentage / 100m;
            attempt.IsPassed = total >= passMarks;

            await _attemptRepository.UpdateAsync(attempt);
            await _answerRepository.SaveChangesAsync();
            await _attemptRepository.SaveChangesAsync();
        }
    }
}
