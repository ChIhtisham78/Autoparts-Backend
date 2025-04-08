using Autopart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models.Settings
{
    public class ContactDetails
    {
        public string Contact { get; set; }
        public List<Social> Socials { get; set; }
        public string Website { get; set; }
        public string EmailAddress { get; set; }
        public Location Location { get; set; }
    }
}
