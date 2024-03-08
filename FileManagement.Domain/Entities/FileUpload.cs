//using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FileManagement.Domain.Entities
{
    public class FileUpload
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; } // لا يتطلب ادخاله من اليوزر

        [MaxLength]
        public string? Path { get; set; } // لا يتطلب ادخاله من اليوزر

        [MaxLength(50)]
        public string? FileType { get; set; } // لا يتطلب ادخاله من اليوزر

        [Required]
        [MaxLength(50)]
        public required string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        [MaxLength]
        [Required]
        public required string CreatedBy { get; set; }

        public FileUpload()
        {
            //Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }
    }
}