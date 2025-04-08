using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class OrderStatusDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public int? Serial { get; set; }

        public string Color { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Language { get; set; }
        public string[] TranslatedLanguages { get; set; }
    }
}
