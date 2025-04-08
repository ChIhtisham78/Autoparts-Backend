namespace Autopart.Application.Models
{
	public class YearlyTotalOrdersByStatus
	{
		public int Pending { get; set; }
		public int Processing { get; set; }
		public int Complete { get; set; }
		public int Cancelled { get; set; }
		public int Refunded { get; set; }
		public int Failed { get; set; }
		public int LocalFacility { get; set; }
		public int OutForDelivery { get; set; }
	}
	public class UpdatePhoneNumberRequest
	{
		public int Id { get; set; }
		public string PhoneNumber { get; set; } = string.Empty;
	}

}
