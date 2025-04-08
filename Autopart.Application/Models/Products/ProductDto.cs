namespace Autopart.Application.Models.Products
{
	public class ProductDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }
		public int? EngineId { get; set; }
		public int? ModelId { get; set; }

		public string Description { get; set; }
		public int? SubCategoryId { get; set; }

		public int? TypeId { get; set; }
		public int? ManufacturerId { get; set; }
		//public int? AuthorId { get; set; }

		public decimal? Price { get; set; }

		public int? ShopId { get; set; }

		public decimal? SalePrice { get; set; }

		public string Language { get; set; }

		public decimal? MinPrice { get; set; }

		public decimal? MaxPrice { get; set; }
		public List<int>? TagIds { get; set; }
		public int? CategoryId { get; set; }

		public string Sku { get; set; }

		public int? Quantity { get; set; }

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

		public int? PromotionalSliderId { get; set; }

		public int? Year { get; set; }

		public string Model { get; set; }

		public int? Mileage { get; set; }

		public string Grade { get; set; }

		public short? Damage { get; set; }

		public string TrimLevel { get; set; }

		public double? Engine { get; set; }

		public string Transmission { get; set; }

		public string Drivetrain { get; set; }

		public string NewUsed { get; set; }

		public string OemPartNumber { get; set; }

		public string PartslinkNumber { get; set; }

		public string HollanderIc { get; set; }

		public string StockNumber { get; set; }

		public string TagNumber { get; set; }

		public double? Location { get; set; }

		public string Site { get; set; }

		public string Vin { get; set; }

		public int? Core { get; set; }

		public string Color { get; set; }
	}
}
