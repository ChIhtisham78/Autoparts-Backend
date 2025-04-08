namespace Autopart.Application.Models
{
    public class ProductCategoryDto
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Slug { get; set; }
        public string Icon { get; set; }

        public int? ParentId { get; set; }

    }
}
