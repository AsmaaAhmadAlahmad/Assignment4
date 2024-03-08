using System.ComponentModel.DataAnnotations;

namespace FileMangementMvcApp.Models
{
    public class LoginModel
    {
        [Required]
        public required string  UserName { get; set; }

        [Required]
       
        public string Password { get; set; }
    }
}
