using Autopart.Domain.Models;

namespace Autopart.Domain.Interfaces
{
	public class ProductJoinDto
	{
		public Product Product { get; set; }
		public Category Category { get; set; }
		public Shop Shop { get; set; }
		public Rating Rating { get; set; }
		public Gallery Gallery { get; set; }
		public PromotionalSlider Promotional { get; set; }
		public Manufacture Manufacturer { get; set; }
		public Setting Setting { get; set; }
		public Image CoverImage { get; set; }
		public Image LogoImage { get; set; }
		public Image ProductImage { get; set; }
		public Address Addresss { get; set; }
		public Engine Engine { get; set; }
		public SubCategoryList SubCategoryList { get; set; }
		public ManufacturerModel ManufacturerModel { get; set; }


	}

}