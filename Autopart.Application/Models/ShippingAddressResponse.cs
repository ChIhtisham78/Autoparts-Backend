using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class ShippingAddressResponse
    {
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string StreetAddress { get; set; }
    }
}
