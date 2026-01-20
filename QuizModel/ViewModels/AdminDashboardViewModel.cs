namespace QuizSystemModel.ViewModels
{
    public class AdminDashboardViewModel
    {
        public long TotalUsers { get; set; }
        public long TotalInstructors { get; set; }
        public long TotalStudents { get; set; }
        public long TotalQuizzes { get; set; }
        public long PendingQuizzes { get; set; }
        public long TotalClasses { get; set; }
        public long TotalSubjects { get; set; }
        public long TotalEducationMediums { get; set; }
        public long PendingClasses { get; set; }
        public long PendingSubjects { get; set; }
        public long PendingInstructors { get; set; }
        public long PendingEducationMediums { get; set; }
        public long PendingProfileUpdates { get; set; }
    }

}
