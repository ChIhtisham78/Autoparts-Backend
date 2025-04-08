namespace Autopart.Application.Models
{
	public class OrdersProductResponse
	{
		public int? OrderId { get; set; }
		public int Id { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }
		public decimal TotalPrice { get; set; }

		public decimal? SalesTax { get; set; }
		public int? Quantity { get; set; }

		public string Description { get; set; }

		public int? TypeId { get; set; }
		public int? CategoryId { get; set; }
		public int? TagId { get; set; }
		public int? AuthorId { get; set; }

		public int? ManufacturerId { get; set; }

		public decimal? Price { get; set; }

		public int? ShopId { get; set; }

		public decimal? SalePrice { get; set; }

		public string Language { get; set; }

		public decimal? MinPrice { get; set; }

		public decimal? MaxPrice { get; set; }

		public string Sku { get; set; }

		public bool? InStock { get; set; }

		public bool? IsTaxable { get; set; }

		public int? ShippingClassId { get; set; }

		public string Status { get; set; }

		public string ProductType { get; set; }

		public string Unit { get; set; }

		public decimal? Height { get; set; }

		public decimal? Width { get; set; }

		public decimal? Length { get; set; }

		public int? ImageId { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? DeletedAt { get; set; }

		public bool? IsDigital { get; set; }

		public bool? IsExternal { get; set; }

		public string ExternalProductUrl { get; set; }

		public string ExternalProductButtonText { get; set; }

		public decimal? Ratings { get; set; }

		public int? TotalReviews { get; set; }

		public string MyReview { get; set; }

		public bool? InWishlist { get; set; }
		public decimal? Total { get; set; }

		public decimal? Amount { get; set; }
		public decimal? Discount { get; set; }
		public int OrderQuantity { get; set; }
		public string? TrackingNo { get; set; }
		public string? DeliveryTime { get; set; }
		public decimal? DeliveryFee { get; set; }

		public virtual ImageDto ImageDto { get; set; }
		public virtual ProductImageDto ProductImageDto { get; set; }
	}

}
