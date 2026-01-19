using QuizSystemModel.Models;
using System.Collections.Generic;

namespace QuizSystemModel.ViewModels
{
    public class GlobalSearchViewModel
    {
        public string Query { get; set; } = string.Empty;
        public List<Instructor> Instructors { get; set; } = new();
        public List<Student> Students { get; set; } = new();
        public List<Quiz> Quizzes { get; set; } = new();
        public List<Subject> Subjects { get; set; } = new();
    }
}
