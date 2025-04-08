using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models.Settings
{
    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string FormattedAddress { get; set; }
    }
}
