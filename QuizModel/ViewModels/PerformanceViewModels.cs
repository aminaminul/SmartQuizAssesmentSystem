namespace QuizSystemModel.ViewModels
{
    public class StudentPerformanceViewModel
    {
        public List<StudentPerformanceItem> Students { get; set; } = new();
        public long? SelectedClassId { get; set; }
        public long? SelectedMediumId { get; set; }
    }

    public class StudentPerformanceItem
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ClassName { get; set; }
        public string? MediumName { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public int QuizzesPassed { get; set; }
        public int QuizzesFailed { get; set; }
        public decimal PassRate { get; set; }
    }

    public class ClassPerformanceViewModel
    {
        public List<ClassPerformanceItem> Classes { get; set; } = new();
        public long? SelectedMediumId { get; set; }
    }

    public class ClassPerformanceItem
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string? MediumName { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public decimal PassRate { get; set; }
    }

    public class MediumPerformanceViewModel
    {
        public List<MediumPerformanceItem> Mediums { get; set; } = new();
    }

    public class MediumPerformanceItem
    {
        public long MediumId { get; set; }
        public string MediumName { get; set; } = string.Empty;
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public decimal PassRate { get; set; }
    }

    public class MyPerformanceViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? ClassName { get; set; }
        public string? MediumName { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public int QuizzesPassed { get; set; }
        public int QuizzesFailed { get; set; }
        public decimal PassRate { get; set; }
        public List<QuizAttemptSummary> RecentAttempts { get; set; } = new();
    }

    public class QuizAttemptSummary
    {
        public long AttemptId { get; set; }
        public string QuizName { get; set; } = string.Empty;
        public string? SubjectName { get; set; }
        public DateTime AttemptDate { get; set; }
        public decimal Score { get; set; }
        public int TotalMarks { get; set; }
        public bool Passed { get; set; }
    }
}
