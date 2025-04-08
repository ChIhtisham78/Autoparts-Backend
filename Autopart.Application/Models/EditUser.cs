using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class EditUser
    {
        public int Id { get; set; }
        public List<AddressFormatDto> Address { get; set; }
    }
}
