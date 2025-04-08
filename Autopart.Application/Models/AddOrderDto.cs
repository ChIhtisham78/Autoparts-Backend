namespace Autopart.Application.Models
{
	public class AddOrderDto
	{
		public int? Id { get; set; }

		public int? CustomerId { get; set; }


		public bool? OrderStatus { get; set; }

		//public int? ShopId { get; set; }
		public int? CouponsId { get; set; }

		public DateTime? CreatedAt { get; set; }

		public int? StatusId { get; set; }
		public string Note { get; set; }
		public string Language { get; set; }

		public List<OrderLineForOrdersAdditionDto> OrderLineForOrdersAdditionDto { get; set; }
		public virtual ShippingAddressDto ShippingAddressDto { get; set; }
		public BillingsAddressDto BillingsAddressDto { get; set; }

	}
}
