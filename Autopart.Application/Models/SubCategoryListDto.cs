namespace Autopart.Application.Models
{
	public class SubCategoryListDto
	{
		public int Id { get; set; }
		public int? CategoryId { get; set; }
		public string Subcategory { get; set; }
		public string CategoryName { get; set; }
		public string Slug { get; set; }
	}

}
