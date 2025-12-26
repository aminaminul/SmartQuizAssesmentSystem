using System.ComponentModel.DataAnnotations;

namespace QuizSystemModel.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public required string Email { get; set; }

    }
}
