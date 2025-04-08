using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class TopRatedProducts
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public decimal? Price { get; set; }

        public decimal? SalePrice { get; set; }


        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public string Sku { get; set; }

        public int? Quantity { get; set; }

        public bool? InStock { get; set; }

        public string Status { get; set; }

        public decimal? Ratings { get; set; }

        public int? TotalReviews { get; set; }

        public string MyReview { get; set; }

        public string OriginalUrl { get; set; }
        public int? Total { get; set; }
        public int? ImageId { get; set; }
    }
}
