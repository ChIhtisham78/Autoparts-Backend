using Autopart.Application.Models.Dto;
using Autopart.Application.Models.Products;

namespace Autopart.Application.Models
{
	public class AddOrderResponse
	{
		public int Id { get; set; }
		public int? OrderNumber { get; set; }
		public int? CustomerId { get; set; }
		public string? CustomerContact { get; set; }
		public string? CustomerName { get; set; }
		public decimal? Amount { get; set; }
		public decimal? SalesTax { get; set; }
		public decimal? PaidTotal { get; set; }
		public decimal? Total { get; set; }
		public string? Note { get; set; }
		public string? Language { get; set; }
		public int? CouponId { get; set; }
		//public int? ShopId { get; set; }
		public decimal? Discount { get; set; }
		public string? PaymentGateway { get; set; }
		public string? AlteredPaymentGateway { get; set; }
		public ShippingAddressResponse ShippingAddress { get; set; }
		public ShippingAddressResponse BillingAddress { get; set; }
		public List<ProductDto> Products { get; set; }
		//public UserResponseForOrder User { get; set; }  
		public ShopDto Shop { get; set; }
		public object? PaymentIntent { get; set; }
	}
}
