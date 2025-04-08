using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IShippingsRepository : IRepository<Shipping>
	{
		void AddShippings(Shipping shipping);
		void AddShippingsAddress(ShippingAddress shippingAddress);
		Task<List<Shipping>> GetShippings();
		Task<Shipping> GetShippingsById(int id);
		Task<Svcrelation> GetSVCRelationByShopIdAndSizeAsync(int shopId, string size);

		Task<Shipping> GetByIdAsync(int id);
		Task<decimal> GetTaxRateForUserAddressAsync(ShippingAddress shippingAddress);

		Task<ShippingAddress> GetByShippingIdAsync(int id);
		void UpdateShippings(Shipping shipping);
		void UpdateShippingsAddress(ShippingAddress shippingAddress);
		Task<bool> RemoveShippings(int id);
	}
}
