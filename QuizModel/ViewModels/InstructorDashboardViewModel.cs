namespace QuizSystemModel.ViewModels
{
    public class InstructorDashboardViewModel
    {
        public long TotalQuizzes { get; set; }

        public long TotalStudents { get; set; }
        public long TotalInstructors { get; set; }

        public long TotalEducationMediums { get; set; }
        public long TotalClasses { get; set; }
        public long TotalSubjects { get; set; }

        public double StudentPerformanceAvg { get; set; }
        public double ClassPerformanceAvg { get; set; }
        public double EducationMediumPerformanceAvg { get; set; }
    }
}
