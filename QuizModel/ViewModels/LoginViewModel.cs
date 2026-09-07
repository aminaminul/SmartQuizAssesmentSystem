using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        // Expected role — passed from role card on Home page
        public string? Role { get; set; }
    }
}
