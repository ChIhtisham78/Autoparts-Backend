namespace Autopart.Application.Models
{
	public class ManufactureModelDto
	{
		public int Id { get; set; }
		public int? ManufacturerId { get; set; }
		public string ManufacturerName { get; set; }
		public string Model { get; set; }
		public string Slug { get; set; }
	}
}
