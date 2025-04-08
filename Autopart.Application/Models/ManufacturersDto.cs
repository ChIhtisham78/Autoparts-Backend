namespace Autopart.Application.Models
{
    public class ManufacturersDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Language { get; set; }

        public string TranslatedLanguages { get; set; }

        public int? ProductsCount { get; set; }

        public bool? IsApproved { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ImageId { get; set; }

        public int? TypeId { get; set; }

        public int? SocialId { get; set; }

        public int? BannerId { get; set; }

        public int? PromotionalSliderId { get; set; }
        public string? ImageOriginalUrl { get; set; }
        public string? ImageThumbnailUrl { get; set; }

    }
}
