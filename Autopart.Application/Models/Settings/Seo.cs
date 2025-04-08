using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models.Settings
{
    public class Seo
    {
        public object OgImage { get; set; }
        public object OgTitle { get; set; }
        public object MetaTags { get; set; }
        public object MetaTitle { get; set; }
        public object CanonicalUrl { get; set; }
        public object OgDescription { get; set; }
        public object TwitterHandle { get; set; }
        public object MetaDescription { get; set; }
        public object TwitterCardType { get; set; }
        // Add other properties as needed
    }
}
