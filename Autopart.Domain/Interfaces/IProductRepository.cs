
using Autopart.Domain.CommonDTO;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Domain.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> CreateProductAsync(Product product);
        Task<IEnumerable<ProductJoinDto>> GetProductsBelowStockAsync(int threshold);
        Task<Svcrelation> GetSVCRelationByShopIdAndSizeAsync(int shopId, string size);
        Task<List<Tag>> GetTagsByProductIdAsync(int productId);
        Task<SubCategoryList> GetSubCategoryByIdAsync(int id);
        Task<ManufacturerModel> GetManufactureModelByIdAsync(int id);
        Task<Engine> GetEngineByIdAsync(int id);
        Task<Product> GetWishlistProductAsync(int userId, int productId);
        Task<IEnumerable<GetWishlistProductsDto>> GetWishlistProductsByUserIdAsync(int userId);
        Task<IEnumerable<Product>> GetWishlistProductsByProductIdAsync(int productId);
        Task<bool> DeleteWishlistProductsByProductIdAsync(int productId);
        Task AddProductToWishlistAsync(int userId, int productId);
        Task<List<ProductTag>> CreateProductTagsAsync(List<ProductTag> productTags);
        //Task<Models.Type> AddTypeAsync(Models.Type type);
        Task<Category> AddCategoryAsync(Category category);
        Task<Author> AddAuthorAsync(Author author);
        Task<Shop> AddShopAsync(Shop shop);
        Task<Manufacture> AddManufactureAsync(Manufacture manufacture);
        Task<Address> GetShopAddressById(int productId);
        Task<List<Category>> GetCategorieAsync();
        Task<List<Shop>> GetShopsWithCategoriesAndProductsAsync();
        Task<Setting> GetSettingByProductId(int productId);
        Task<Rating> GetRatingByProductId(int productId);
        Task<Gallery> GetGalleryByProductId(int productId);
        Task<PromotionalSlider> GetPromotionalByProductId(int productId);
        Task<Shop> GetShopByProductId(int productId);
        Task<List<Category>> GetCategoriesByProduct(List<int> shopIds = null);
        Task<List<Tag>> GetTagsByProduct();
        Task<List<Author>> GetAuthorsByProduct();
        Task<List<Manufacture>> GetManufacturersByProduct();
        Task<Image> GetImageByProductId(int productId);
        Task<List<Shop>> GetShopsByProduct();
        Task<Tag> AddTagAsync(Tag tag);
        Task<Image> AddImage(Image image);
        Task<Gallery> CreateGalleryAsync(Gallery gallery);
        Task<(IEnumerable<ProductJoinDto> Products, int TotalCount)> GetProductsAsync(GetProductsDto getProductsDto);
        Task<IEnumerable<ProductJoinDto>> GetProductBySlugAsync(string slug);
        Task<Product> GetProductByIdAsync(int id);
        Task<Category> GetCategoryByIdAsync(int ProductId);
        Task UpdateProductAsync(Product product);
        Task<Image> UpdateImage(Image image);
        //Task UpdateTypeAsync(Models.Type type);
        Task UpdateCategoryAsync(Category category);
        Task UpdateAuthorAsync(Author author);
        Task UpdateTagAsync(Tag tag);
        Task UpdateGalleryAsync(Gallery gallery);
        Task<bool> RemoveProductAsync(int id);
        Task DeleteImages(int productId);
        Task DeleteGalleriesAsync(int productId);
        Task DeleteProductTagsByProductIdAsync(int productId);
        Task<IEnumerable<Product>> GetPopularProductsAsync(int? shopId = null, List<int> shopIds = null);
        Task<IEnumerable<Product>> GetBestSellingProductsAsync(int? shopId = null, List<int> shopIds = null);
        Task<IEnumerable<Product>> GetDraftProductsAsync();
        Task<IEnumerable<Product>> GetProductsStockAsync();
        Task<IEnumerable<ProductJoinDto>> GetProductsByShopId(int shopId);
        Task<int> GetProductByCategoryAsync(int categoryId);
        Task<int> GetProductByShopAsync(int shopId);
        Task<IEnumerable<Product>> GetTopRatedProductsAsync(int? shopId = null, List<int> shopIds = null);
        Task<Image?> GetImageById(int id);
        Task<Shop> GetShopById(int id);
        Task<string> GetManufacturerNameById(int? manufacturerId);
        Task<string> GetOrCreateEngineIdAsync(int? engineId);
        Task<string> GetModelNameById(int? modelId);
        Task<string> GetSubCategoryNameById(int? subCategoryId);
        Task<string> GetCategoryNameById(int? categoryId);
        Task<bool> SlugExistsAsync(string slug);
    }
}
