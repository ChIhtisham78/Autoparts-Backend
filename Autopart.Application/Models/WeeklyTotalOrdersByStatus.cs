using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class WeeklyTotalOrdersByStatus
    {
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int Complete { get; set; }
        public int Cancelled { get; set; }
        public int Refunded { get; set; }
        public int Failed { get; set; }
        public int LocalFacility { get; set; }
        public int OutForDelivery { get; set; }
    }
}
