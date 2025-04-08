using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class OrderLineForOrdersAdditionDto
    {
        public int Id { get; set; }

        public int? ProductId { get; set; }

        //public int? OrderId { get; set; }

        public int? Quantity { get; set; }

        public decimal? Amount { get; set; }

        //public decimal? SalesTax { get; set; }

        public decimal? SubTotal { get; set; }

        //public decimal? PaidTotal { get; set; }

        //public decimal? Discount { get; set; }

        //public decimal? DeliveryFee { get; set; }

        public string DeliveryTime { get; set; }

        //public string Language { get; set; }
    }
}
