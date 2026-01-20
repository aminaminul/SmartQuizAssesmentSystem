using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class StudentProfileUpdateViewModel
    {
        public long StudentId { get; set; }
        public long UserId { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "First Name must be at least 2 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name must be at least 2 characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid Last Name")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required"), EmailAddress(ErrorMessage = "Invalid Email Format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{5,}@[a-zA-Z0-9-]{3,}\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Phone Number is required"), Phone(ErrorMessage = "Invalid Phone Format")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Education Medium is required")]
        [Display(Name = "Education Medium")]
        public long? EducationMediumId { get; set; }

        [Required(ErrorMessage = "Class is required")]
        [Display(Name = "Class")]
        public long? ClassId { get; set; }

        public string? EducationMediumName { get; set; }

        public string? ClassName { get; set; }
    }
}
