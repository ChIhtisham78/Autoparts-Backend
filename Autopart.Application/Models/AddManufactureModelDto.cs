namespace Autopart.Application.Models
{
	public class AddManufactureModelDto
	{
		public int Id { get; set; }
		public int? ManufacturerId { get; set; }
		public string Model { get; set; }
		public string Slug { get; set; }
	}
}
