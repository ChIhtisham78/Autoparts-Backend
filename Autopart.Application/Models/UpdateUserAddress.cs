using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class UpdateUserAddress
    {
        public int Id { get; set; }
        //public string Title { get; set; }
        //public string Type { get; set; }
        //public short? IsDefault { get; set; }
        public AddressFormatDto Address { get; set; }
    }
}
