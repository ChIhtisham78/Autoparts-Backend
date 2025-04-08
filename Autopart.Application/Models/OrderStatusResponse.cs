using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class OrderStatusResponse
    {
        public string Name { get; set; }

        public int? Serial { get; set; }

        public string Color { get; set; }
        public string Language { get; set; }

    }
}
