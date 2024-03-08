using FileManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileManagement.Data
{
    public class FileManagementDBContext : DbContext
    {

        public FileManagementDBContext()
        {
            
        }
        public FileManagementDBContext(DbContextOptions<FileManagementDBContext> options):base(options) 
        {

        }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer();
            base.OnConfiguring(optionsBuilder);
        }
    }
}