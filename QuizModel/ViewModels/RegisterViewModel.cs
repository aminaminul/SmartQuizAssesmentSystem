using QuizSystemModel.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }
        public string? RegistrationType { get; set; }
        
        
        // Student
        public EducationMedium? EducationMediumId { get; set; }
        public Class? ClassId { get; set; }

        
        
        // Instructor
        public string PhoneNumber { get; set; }
        public string HscPassingInstitute { get; set; }
        public long? HscPassingYear { get; set; }
        public string HscGrade { get; set; }
    }
}
