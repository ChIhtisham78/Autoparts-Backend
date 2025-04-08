using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autopart.Application.Models
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string UserName { get; set; }

        public bool IsActive { get; set; }

        public virtual ImageDto? ImageDto { get; set; }
        public virtual AddressDto? AddressDto { get; set; }
        //public virtual CustomerProfile CustomerProfile { get; set; }
    }

    //public class CustomerProfile
    //{
    //    public int? ImageId { get; set; }
    //    public string? OriginalUrl { get; set; }
    //    public string? ThumbnailUrl { get; set; }
    //}
}
