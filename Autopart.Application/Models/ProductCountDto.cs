using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class ProductCountDto
    {
        public int? ShopId { get; set; }
        public string ShopName { get; set; }
        public int NumberOfProducts { get; set; }

        public int? CatogeryId { get; set; }
        public string CategoryName { get; set; }
        public int NumberOfProduct { get; set; }
        //public List<CategorySummeryDto> ProductsByCategory { get; set; }
        //public List<ShopSummeryDto> ProductsByShop { get; set; }

    }
}
