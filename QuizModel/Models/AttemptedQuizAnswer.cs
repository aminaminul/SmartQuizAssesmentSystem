using QuizSystemModel.Models;

public class AttemptedQuizAnswer
{
    public long Id { get; set; }

    public long QuizAttemptId { get; set; }
    public QuizAttempt QuizAttempt { get; set; }

    public long QuestionBankId { get; set; }
    public QuestionBank QuestionBank { get; set; }

    public string? SelectedOption { get; set; }
    public decimal Score { get; set; }
}
