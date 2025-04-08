using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class CategoryProductCountDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ShopName { get; set; }
        public int ProductCount { get; set; }
    }

    public class ShopCategoryProductCountDto
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public List<CategoryProductCountDto> Categories { get; set; }
    }
}
