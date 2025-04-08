using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Models
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string CoverImage { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
