namespace Autopart.Application.Models
{
	public class AddEngineDto
	{
		public int Id { get; set; }

		public int? Year { get; set; }

		public int? CategoryId { get; set; }

		public int? SubcategoryId { get; set; }

		public int? ManufacturerId { get; set; }

		public int? ModelId { get; set; }

		public string Engine1 { get; set; }

		public string HollanderCode { get; set; }

	}
}
