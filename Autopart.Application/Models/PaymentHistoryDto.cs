using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class PaymentHistoryDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public string VendorId { get; set; }

        public string PaymentId { get; set; }

        public string Status { get; set; }

        public decimal? ChargedAmount { get; set; }

        public int? OrderId { get; set; }
    }
}
