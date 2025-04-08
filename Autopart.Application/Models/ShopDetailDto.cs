namespace Autopart.Application.Models.Dto
{
	public class ShopDetailDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int ProductCount { get; set; }
		public int OrdersCount { get; set; }
		public string OwnerName { get; set; }
		public int? OwnerId { get; set; }
		public bool IsActive { get; set; }
		public bool? Status { get; set; }
		public string Logo { get; set; }
		public string CoverImage { get; set; }
		public string Slug { get; set; }

		public string Description { get; set; }

		//public virtual ImageDto Image { get; set; }

		//public virtual AddressDto Address { get; set; }
	}
}
