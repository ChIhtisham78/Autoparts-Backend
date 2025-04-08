using Autopart.Domain.CommonDTO;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IShopRepository : IRepository<Shop>
	{
		Task<Shop> AddShop(Shop shop);
		Task AddAddress(Address address);
		Task AddImage(Image image);
		Task AddSocial(Social social);
		Task AddBalance(Balance balance);
		Task AddSetting(Setting setting);
		//Task<List<ShopSocial>> AddShopSocial(List<ShopSocial> shopSocials);
		Task<List<ShopWithDetails>> GetShopsWithDetailsAsync();

		Task<Shop> GetShopById(int Id);
		Task<Shop> GetShopBySlug(string slug);
		Task<int> GetOrdersCount(int shopId);
		Task<int> GetProductCount(int shopId);
		Task<string> GetOwnerName(int? ownerId);
		Task<string> GetImageById(int? imageId);
		Task<AspNetUser?> GetShopByUserId(int ownerId);
		Task<AspNetUser?> GetShopByUserRoleId(int ownerId);
		Task<List<Shop>> GetShops();
		Task<Address> GetShopAddressById(int shopId);
		Task<Setting> GetSettingByShopId(int shopId);
		Task<Social> GetSocialByShopId(int shopId);
		Task<Balance> GetBalanceByShopId(int shopId);
		//Task<List<ShopDetailDto>> GetShopIa();
		Task<List<Shop>> GetShopsByOwnerIdAsync(int ownerId);
		Task<int> GetShopsCount(int? vendorId = null);


		//Task<Shop> GetShopById(int id);
		Task<Shop> UpdateShop(Shop shop);
		Task<Address> UpdateAddress(int shopId, Address address);
		Task<Balance> UpdateBalance(Balance balance);
		Task<Image> UpdateImage(Image image);
		Task<Social> UpdateSocial(Social social);
		Task<Setting> UpdateSetting(Setting setting);
		Task DeleteAddresses(int shopId);
		Task DeleteBalances(int shopId);
		Task DeleteImages(int shopId);
		Task DeleteSocials(int shopId);
		Task DeleteSettings(int shopId);
		Task DeleteOrders(int shopId);
		Task DeleteCoupons(int shopId);
		Task DeleteAttribute(int shopId);
		Task<bool> DeleteShop(int id);
		Task<Shop> ApproveShop(int id);
		Task<Shop> DisapproveShop(int id);
		Task<bool> SlugExistsAsync(string slug);
		Task<Image?> GetImagesById(int? imageId);
	}
}
