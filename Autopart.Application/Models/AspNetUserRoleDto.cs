using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class AspNetUserRoleDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } // Assuming Role has a name property
        public ICollection<AspNetUserDto> Users { get; set; } // Flatten users or only include necessary properties
    }

    public class AspNetUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
