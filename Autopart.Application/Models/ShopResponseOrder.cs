namespace Autopart.Application.Models
{
	public class ShopResponseOrder
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int? OwnerId { get; set; }
		public bool IsActive { get; set; }
		public bool? Status { get; set; }
		public string Slug { get; set; }

		public string Description { get; set; }
	}
}
