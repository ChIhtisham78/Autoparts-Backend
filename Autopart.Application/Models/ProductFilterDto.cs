using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Application.Models
{
	public class ProductFilterDto
	{
		public int? shopId { get; set; }
		public int? categoryId { get; set; }
		public int? subCategoryId { get; set; }
		public string? name { get; set; }
		public string? model { get; set; }
		public string? vin { get; set; }
		public string? make { get; set; }
		public int? manufacturerId { get; set; }
		public int? year { get; set; }
		public int? engineId { get; set; }
		public int? modelId { get; set; }
		public bool isHome { get; set; }
		public OrderBy orderBy { get; set; } = OrderBy.Ascending;
		public SortedByProductName sortedByProductName { get; set; } = SortedByProductName.ProductName;
	}

}
