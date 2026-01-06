using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class InstructorProfileUpdateViewModel
    {
        public long InstructorId { get; set; }
        public long UserId { get; set; }

        [Required]
        [StringLength(70, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; } = null!;

        [Required, EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{5,}@[a-zA-Z0-9-]{3,}\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required, Phone]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 10)]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid Institute Name")]
        public string HscPassingInstitute { get; set; } = null!;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Invalid Passing Year")]
        public long? HscPassingYear { get; set; }

        [Required]
        [RegularExpression(@"^(4\.[0-9]{2}|5\.00)$", ErrorMessage = "GPA must be between 4.00 and 5.00")]
        public string HscGrade { get; set; } = null!;

        [Required(ErrorMessage = "Education Medium is required")]
        public long? EducationMediumId { get; set; }
    }
}
