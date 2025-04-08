namespace Autopart.Application.Models
{
	public class UpdateTagDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Slug { get; set; }
		public string Details { get; set; }

	}
	public class RefundResponse
	{
		public int Id { get; set; }
		public string RefundReason { get; set; }
		public string CustomerEmail { get; set; }
		public decimal Amount { get; set; }
		public string TrackingNumber { get; set; }
		public string Status { get; set; }
		public DateTime OrderDate { get; set; }
	}

}
