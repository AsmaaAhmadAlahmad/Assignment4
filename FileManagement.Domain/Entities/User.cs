using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManagement.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public required string UserName { get; set; }

        public required string Password { get; set; }

        public User()
        {
            Id = Guid.NewGuid();
        }
    }
}
