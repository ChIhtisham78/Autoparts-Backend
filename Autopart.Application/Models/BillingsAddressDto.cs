namespace Autopart.Application.Models
{
	public class BillingsAddressDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }
		public short? IsDefault { get; set; }
		public int? OrderId { get; set; }
		public string Zip { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string StreetAddress { get; set; }
		public DateTime? CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
