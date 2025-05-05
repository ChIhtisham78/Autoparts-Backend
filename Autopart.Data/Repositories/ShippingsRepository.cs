using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class ShippingsRepository : IShippingsRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public ShippingsRepository(autopartContext context)
		{
			_context = context;
		}

		public async Task<Svcrelation> GetSVCRelationByShopIdAndSizeAsync(int shopId, string size)
		{
			var svcRelation = await _context.Svcrelations.Where(x => x.ShopId == shopId && x.Size == size).FirstOrDefaultAsync();
			return svcRelation ?? new Svcrelation();
		}

        public async Task<decimal> GetTaxRateForUserAddressAsync(ShippingAddress shippingAddress)
        {
            var taxRate = 0.0m;
            if (shippingAddress != null && !string.IsNullOrEmpty(shippingAddress.State))
            {
                taxRate = await _context.Taxes
                    .Where(t => t.State == shippingAddress.State)
                    .Select(t => (decimal?)t.Rate)
                    .FirstOrDefaultAsync() ?? 0;
            }

            return taxRate;
        }



        public void AddShippings(Shipping shipping)
		{
			_context.Shippings.Add(shipping);
		}

		public void AddShippingsAddress(ShippingAddress shippingAddress)
		{
			_context.ShippingAddresses.Add(shippingAddress);
		}

		public async Task<List<Shipping>> GetShippings()
		{
			var shipping = await _context.Shippings
                .Include(s => s.ShippingAddresses)
                .ToListAsync();

			return shipping;
		}

		public async Task<Shipping> GetShippingsById(int id)
		{
			return await _context.Shippings
				.Include(s => s.ShippingAddresses)
				.FirstOrDefaultAsync(s => s.Id == id) ?? new Shipping();
		}

		public async Task<Shipping> GetByIdAsync(int id)
		{
			return await _context.Shippings.FirstOrDefaultAsync(s => s.Id == id) ?? new Shipping();
		}

		public async Task<ShippingAddress> GetByShippingIdAsync(int id)
		{
			return await _context.ShippingAddresses.FirstOrDefaultAsync(s => s.ShippingId == id) ?? new ShippingAddress();
		}

		public void UpdateShippings(Shipping shipping)
		{
			_context.Shippings.Update(shipping);
		}

		public void UpdateShippingsAddress(ShippingAddress shippingAddress)
		{
			_context.ShippingAddresses.Update(shippingAddress);
		}

		public async Task<bool> RemoveShippings(int id)
		{
			var shipping = await _context.Shippings.Include(s => s.ShippingAddresses).FirstOrDefaultAsync(s => s.Id == id);
			if (shipping == null)
			{
				return false;
			}

			_context.ShippingAddresses.RemoveRange(shipping.ShippingAddresses);
			_context.Shippings.Remove(shipping);

			return true;
		}
	}
}
