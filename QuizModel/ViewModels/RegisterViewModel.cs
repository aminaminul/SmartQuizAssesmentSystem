using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First Name is required.")]
        public required string FirstName { get; set; }
        
        [Required(ErrorMessage = "First Name is required.")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public required string ConfirmPassword { get; set; }
        [Required]
        public string Role { get; set; }

        public InstructorDetails InstructorInfo { get; set; } = new();
    }
    public class InstructorDetails
    {
        public string Qualifications { get; set; }
        public string Expertise { get; set; }
        public string Phone { get; set; }
    }
}
