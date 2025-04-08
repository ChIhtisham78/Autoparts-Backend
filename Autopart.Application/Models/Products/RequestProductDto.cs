namespace Autopart.Application.Models.Products
{
    public class RequestProductDto
    {
        public ProductDto ProductDto { get; set; }
        public ImageDto ImageDto { get; set; }
        public ProductGalleryImageDto ProductGDto { get; set; }
        public TypeDto TypeDto { get; set; }
        public ProductCategoryDto ProductCDto { get; set; }
        public ProductAuthorDto ProductADto { get; set; }
        public ProductTagDto ProductTDto { get; set; }
    }

}
