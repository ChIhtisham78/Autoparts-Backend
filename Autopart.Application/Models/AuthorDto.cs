namespace Autopart.Application.Models
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public string Quote { get; set; }

        public bool? IsApproved { get; set; }

        public string Slug { get; set; }

        public int? ProductsCount { get; set; }

        public string? Born { get; set; }

        public string? Death { get; set; }

        public string Languages { get; set; }

        public int? ImageId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
