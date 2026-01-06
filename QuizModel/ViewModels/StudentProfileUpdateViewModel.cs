using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class StudentProfileUpdateViewModel
    {
        public long StudentId { get; set; }
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

        [Required(ErrorMessage = "Education Medium is required")]
        public long? EducationMediumId { get; set; }

        [Required(ErrorMessage = "Class is required")]
        public long? ClassId { get; set; }
    }
}
