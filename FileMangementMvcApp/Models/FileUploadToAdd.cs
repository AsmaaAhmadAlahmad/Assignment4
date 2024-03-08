using System.ComponentModel.DataAnnotations;

namespace FileMangementMvcApp.Models
{
    public class FileUploadToAdd
    {
       
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

       
    }
}
