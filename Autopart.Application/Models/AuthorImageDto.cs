using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Models
{
    public class AuthorImageDto
    {
        public IFormFile CoverImage { get; set; }
    }
}
