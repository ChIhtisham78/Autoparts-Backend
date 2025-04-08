using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class ShopSummeryDto
    {
        public int? Id { get; set; }
        public string ShopName { get; set; }
        public int NumberOfProducts { get; set; }
    }
}
