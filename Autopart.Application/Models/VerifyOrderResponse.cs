namespace Autopart.Application.Models
{
	public class VerifyOrderResponse
	{
		// Fields included in the response payload
		public decimal TotalTax { get; set; }
		public decimal ShippingCharge { get; set; }
		public List<string> UnavailableProducts { get; set; } = new List<string>();
	}
	public class WishlistRequest
	{
		public int UserId { get; set; }
		public int ProductId { get; set; }
	}


}
