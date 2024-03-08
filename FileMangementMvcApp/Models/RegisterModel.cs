using System.ComponentModel.DataAnnotations;

namespace FileMangementMvcApp.Models
{
    public class RegisterModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d).+", ErrorMessage = "Password must contain letters and numbers.")]
        public required string Password { get; set; }

        [Required]
        [Compare("Password",ErrorMessage = "Confirm Password doesn't match, try again!")]
        public required string ConfirmPassword { get; set; }
    }
}
