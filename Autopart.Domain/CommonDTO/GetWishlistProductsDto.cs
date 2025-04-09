using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autopart.Domain.Models;

namespace Autopart.Domain.CommonDTO
{
    public class GetWishlistProductsDto
    {
        public UserWishlist UserWishlist { get; set; }
        public Product Product { get; set; }
        public Image Image { get; set; }
    }
}
