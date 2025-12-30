using QuizSystemModel.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizSystemModel.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(70, MinimumLength = 2, ErrorMessage = "First Name Must Be At Least 2 Characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2, ErrorMessage = "Last Name Must Be At Least 2 Characters")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$", ErrorMessage = "Invalid First Name")]
        public string LastName { get; set; }
        
        [Required, EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]{5,}@[a-zA-Z0-9-]{3,}\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        
        
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        
        [DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }

        
        public string? Role { get; set; }
        public string? RegistrationType { get; set; }


        // Student
        public long? EducationMediumId { get; set; }
        public EducationMedium? EducationMedium { get; set; }
        public long? ClassId { get; set; }  
        public Class? Class { get; set; }



        // Instructor
        [Required, Phone]
        [RegularExpression(@"^\+?\d{10,15}$",ErrorMessage = "Invalid Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Invalid Institute Name")]
        [RegularExpression(@"^[A-Za-z][A-Za-z .\-]*$",ErrorMessage = "Invalid Institute Name")]
        public string HscPassingInstitute { get; set; }

        [Required]
        [Range(1900, 2100, ErrorMessage = "Invalid Passing Year")]
        public long? HscPassingYear { get; set; }
        
        [Required]
        [RegularExpression(@"^(5(\.0{1,2})?|[0-4](\.\d{1,2})?)$",ErrorMessage = "Invalid GPA")]
        public string HscGrade { get; set; }
    }
}