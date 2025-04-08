using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class UserResponseForOrder
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class ChangePasswordRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

}
