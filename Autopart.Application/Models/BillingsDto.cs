namespace Autopart.Application.Models
{
	public class BillingsDto
	{
		public int Id { get; set; }
		public int? OrderId { get; set; }
		public int? Amount { get; set; }
		public string Type { get; set; }
		public string TrackingNo { get; set; }
		public bool? Global { get; set; }
		public DateTime? CreatedAt { get; set; }
		public virtual BillingsAddressDto? BillingsAddressDto { get; set; }
	}
}
