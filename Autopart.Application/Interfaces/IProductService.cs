using Autopart.Application.Models;
using Autopart.Application.Models.Products;
using Autopart.Application.Options;
using Autopart.Domain.CommonDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Autopart.Domain.Enum.EnumAndConsonent;

namespace Autopart.Application.Interfaces
{
	public interface IProductService
	{
		Task<IEnumerable<ProductDtoResponse>> GetWishlistProductsAsync(int userId);
		Task<bool> IsProductInWishlistAsync(int userId, int productId);
		Task<IEnumerable<ProductDtoResponse>> GetWishlistProductsByProductIdAsync(int productId);
		Task<bool> DeleteWishlistProductsByProductIdAsync(int productId);
		Task AddProductToWishlistAsync(int userId, int productId);
		Task<ProductDto> CreateProductAsync(ProductRequestDto productRequestDto);
		Task<IEnumerable<ProductDtoResponse>> GetLowStockProductsAsync(int lowStockThreshold);
		Task<(IEnumerable<ProductDtoResponse> Products, int TotalCount)> GetProductsAsync(GetProductsDto getProductsDto);
        Task<ProductDtoResponse> GetProductByIdAsync(int id);
		//Task<ProductDto> GetProductBySlugAsync(string slug);
		Task<ProductDtoResponse> GetProductBySlugAsync(string slug);
		Task<string> UploadImage(IFormFile image);
		Task UpdateProductAsync(ProductRequestDto productRequestDto);
		Task<bool> RemoveProductAsync(int id);
		Task<IEnumerable<ProductDtoResponse>> GetPopularProductsAsync(int? shopId = null, int? vendorId = null);
		Task<IEnumerable<ProductDtoResponse>> GetBestSellingProductsAsync(int? shopId = null, int? vendorId = null);
		Task<IEnumerable<ProductDtoResponse>> GetDraftProductsAsync();
		Task<IEnumerable<ProductDto>> GetProductsStockAsync();
		Task<List<LookupDto>> GetCategoriesByProduct(int? vendorId = null);
		Task<List<LookupDto>> GetTagsByProduct();
		Task<List<LookupDto>> GetAuthorsByProduct();
		Task<List<LookupDto>> GetManufacturersByProduct();
		Task<List<LookupDto>> GetShopsByProduct();
		Task<IEnumerable<ProductDtoResponse>> GetProductsByShopIdAsync(int shopId);
		Task<FileUploadResult> UploadFileAsync(IFormFile file, int shopId);
		Task<ActionResult<IEnumerable<ProductCountDto>>> GetProductsSummaryAsync();
        Task<List<CategoryProductCountDto>> GetShopsWithCategoryProductCountsAsync(GetProductsDto getProductsDto);
		Task<IEnumerable<ProductDtoResponse>> GetTopRatedProductsAsync(int? shopId = null, int? vendorId = null);
		Task<MemoryStream> ExportProducts(int shopId);
	}
}
