using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class ReviewDto
    {
        public int ProductId { get; set; }
        public int? OrderId { get; set; }
        public int Rating { get; set; }
        public string Comments { get; set; }


    }
}
