namespace Autopart.Application.Models
{
    public class BannerDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int? TypeId { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ImageDto Image { get; set; }
    }
}
