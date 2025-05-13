using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Domain.CommonDTO
{
    public class GetEnginesDTO
    {
        public int? year { get; set; }
        public int? categoryId { get; set; }
        public int? subcategoryId { get; set; }
        public int? manufacturerId { get; set; }
        public int? modelId { get; set; }
    }
}
