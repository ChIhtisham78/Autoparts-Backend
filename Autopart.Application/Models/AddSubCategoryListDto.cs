namespace Autopart.Application.Models
{
	public class AddSubCategoryListDto
	{
		public int Id { get; set; }
		public int? CategoryId { get; set; }
		public string Slug { get; set; }
	}
}
