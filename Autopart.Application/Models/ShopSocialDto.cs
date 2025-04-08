using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class ShopSocialDto
    {
        public int? Id { get; set; }

        public string Type { get; set; }

        public string Link { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        //public List<ShopSocialDto> Socials { get; set; }
    }
}

