using QuizSystemModel.Models;

namespace QuizSystemModel.ViewModels
{
    public class StudentViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }

        public long? EducationMediumId { get; set; }
        public EducationMedium? EducationMedium { get; set; }

        public long? ClassId { get; set; }
        public Class? Class { get; set; }

        public string? Role { get; set; }
    }
}
