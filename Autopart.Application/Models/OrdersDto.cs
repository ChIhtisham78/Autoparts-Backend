using Autopart.Application.Models.Products;

namespace Autopart.Application.Models
{
	public class OrdersDto
	{
		public int Id { get; set; }
		public int? CustomerId { get; set; }
		public decimal? Discount { get; set; }
		public int? CouponsId { get; set; }
		public int? OrderNumber { get; set; }
		public decimal? Tax { get; set; } // Tax field properly used
		public string PaymentGateway { get; set; } = "stripe";
		public decimal? TotalAmount { get; set; }
		public decimal? Amount { get; set; }
		public DateTime? CreatedAt { get; set; }
		public int? StatusId { get; set; }
		public string? OrderStatus { get; set; }
		public string? PaymentStatus { get; set; }
		public string? CustomerName { get; set; }
		public string? CustomerContact { get; set; }
		public virtual OrdersProductResponse OrdersProductResponse { get; set; }
		public virtual ShippingsDto? ShippingsDto { get; set; }
		public virtual ShippingAddressDto? ShippingAddressDto { get; set; }
		public virtual BillingsDto? BillingsDto { get; set; }
		public virtual BillingsAddressDto? BillingsAddressDto { get; set; }
		public virtual OrderUserDto OrderUserDto { get; set; }
		public virtual CustomerDto? CustomerDto { get; set; }
		public List<OrdersProductResponse> OrderLines { get; set; } = new List<OrdersProductResponse>();
	}
    public class EmailRequest
    {
        public string Email { get; set; }
    }


}
