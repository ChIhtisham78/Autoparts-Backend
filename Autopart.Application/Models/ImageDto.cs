using Microsoft.AspNetCore.Http;


namespace Autopart.Application.Models
{
    public class ImageDto
    {
        public int Id { get; set; }
        //public IFormFile Logo { get; set; }
        //public IFormFile CoverImage { get; set; }

        public string OriginalUrl { get; set; }

        public string ThumbnailUrl { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
