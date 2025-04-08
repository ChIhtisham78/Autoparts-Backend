using Autopart.Application.Models.Products;

namespace Autopart.Application.Models
{
    public class ProductRequestDto
    {
        public ProductDto productDto { get; set; }
        public ProductImageDto imageDto { get; set; }
        public string[] GallaryImageUrls { get; set; }
        //public TypeDto typeDto { get; set; }
        //public ProductCategoryDto categoryDto { get; set; }
        //public ProductAuthorDto authorDto { get; set; }
        //public ProductTagDto tagDto { get; set; }
        //public ProductShopDto shopDto { get; set; }
        //public ProductManufacturerDto manufacturerDto { get; set; }
    }
}
