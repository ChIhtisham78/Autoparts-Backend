namespace Autopart.Application.Models
{
	public class VerifyOrderDto
	{
		// Fields included in the request payload
		public decimal? Amount { get; set; }
		public AddressResponseDto ShippingAddress { get; set; }
	}
	public class AddressResponseDto
	{
		public string Country { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string StreetAddress { get; set; }
	}
}




