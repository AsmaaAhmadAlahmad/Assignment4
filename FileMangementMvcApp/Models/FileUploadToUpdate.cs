using System.ComponentModel.DataAnnotations;

namespace FileMangementMvcApp.Models
{
    public class FileUploadToUpdate
    {
        [Required]
        public required Guid id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Description { get; set; }
    }
  
}
