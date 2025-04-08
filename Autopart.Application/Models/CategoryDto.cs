namespace Autopart.Application.Models
{
	public class CategoryDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }
		public string Size { get; set; }

		public string Language { get; set; }
		public int? ImageId { get; set; }

		public string Icon { get; set; }

		public int? ParentId { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? DeletedAt { get; set; }

		public int? TypeId { get; set; }
		public ImageDto ImageDto { get; set; }
	}
}
