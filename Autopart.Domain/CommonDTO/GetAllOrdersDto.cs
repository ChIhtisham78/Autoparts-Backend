using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Domain.CommonDTO
{
    public class GetAllOrdersDto
    {
        public int? customerId { get; set; }
        public int? orderNumber { get; set; }
        public string? search { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? trackingNumber { get; set; }
    }
}
