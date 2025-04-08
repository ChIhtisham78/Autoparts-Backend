using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Domain.CommonDTO
{
    public class GetProductsDto
    {
        public bool isHome { get; set; }
        public int? shopId { get; set; }
        public int? categoryId { get; set; }
        public string? searchByName { get; set; }
        public int? subCategoryId { get; set; }
        public string? model { get; set; }
        public string? vin { get; set; }
        public string? manufacturer { get; set; }
        public int? modelId { get; set; }
        public int? manufacturerId { get; set; }
        public int? year { get; set; }
        public int? engineId { get; set; }
        public OrderBy? orderBy { get; set; } = OrderBy.Ascending;
        public SortedByProductName? sortedBy { get; set; } = SortedByProductName.ProductName;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public string? name { get; set; }
        public string? make { get; set; }

    }
}
