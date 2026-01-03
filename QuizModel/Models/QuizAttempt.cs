using QuizSystemModel.Models;
public class QuizAttempt
{
    public long Id { get; set; }
    public long QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public long StudentUserId { get; set; }
    public QuizSystemUser StudentUser { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public bool IsSubmitted { get; set; }
    public decimal TotalScore { get; set; }
    public bool IsPassed { get; set; }
    public DateTime LastSavedAt { get; set; }
    public ICollection<AttemptedQuizAnswer> Answers { get; set; } = new List<AttemptedQuizAnswer>();
}
