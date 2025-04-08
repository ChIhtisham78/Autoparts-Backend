namespace Autopart.Application.Models
{
	public class AddressFormatDto
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Type { get; set; }
		public short? IsDefault { get; set; }
		public AddressDetailsDto Address { get; set; }

	}
	public class AddressDetailsDto
	{
		public string StreetAddress { get; set; }
		public string Country { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
	}

}
