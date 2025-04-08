using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Models
{
    public class ProductGalleryImageDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public IFormFile GallaryImage { get; set; }
    }
}
