using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class SendGridSection
    {
        public const string SettingsSection = "SendGridSection";
        public string ApiKey { get; set; }
        public string From { get; set; }
        public string UserName { get; set; }
    }
}
