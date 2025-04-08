namespace Autopart.Application.Models
{
    public class PromotionalSliderDto
    {
        public int Id { get; set; }
        public string OriginalUrl { get; set; }

        public string ThumbnailUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
