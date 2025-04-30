using Autopart.Application.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Autopart.Application.Interfaces
{
	public interface IShopService
	{
		Task<ShopDto> AddShop(ShopDtoRequest shopDtoRequest, int userId);
		Task<List<ShopDto>> GetShops();
		Task<string> UploadImage(IFormFile image);
		Task<ShopDto> GetShop(int Id);
		Task<ShopDto> GetShopBySlug(string slug);
		Task<ShopDto> UpdateShop(int id,ShopDto shopDto);
		Task<bool> DeleteShop(int id);
		Task<ShopDto> ApproveShop(int id);
		Task<ShopDto> DisapproveShop(int id);
		Task<List<string>> GetRolesByUserId(int userId);
		Task<List<ShopDto>> GetShopsByOwnerIdAsync(int ownerId);
	}
}
