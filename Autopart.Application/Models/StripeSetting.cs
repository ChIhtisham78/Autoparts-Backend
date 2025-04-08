using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class StripeSetting
    {
        public const string StripeSettings = "StripeSetting";
        public string PubKey { get; set; }
        public string SecretKey { get; set; }
        public string EndPointKey { get; set; }
        public string SUrl { get; set; }
        public string CUrl { get; set; }
    }
}
