using QuizSystemModel.Models;
using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class StudentAddViewModel
    {
        [Required]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "First Name Must Be At Least 2 Characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name Must Be At Least 2 Characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; }

        [Required, EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{5,}@[a-zA-Z0-9-]{3,}\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required, Phone]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Education Medium is required")]
        [Display(Name = "Education Medium")]
        public long? EducationMediumId { get; set; }
        public EducationMedium? EducationMedium { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public long? ClassId { get; set; }
        public Class? Class { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }
    }
}
