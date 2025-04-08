using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class BillingsRepository : IBillingsRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public BillingsRepository(autopartContext context)
		{
			_context = context;
		}
		public void AddBillings(Billing billing)
		{
			_context.Billings.Add(billing);
		}

		public void AddBillingsAddress(BillingAddress billingAddress)
		{
			_context.BillingAddresses.Add(billingAddress);
		}

		public async Task<List<Billing>> GetBillings()
		{
			return await _context.Billings
				.Include(s => s.BillingAddresses)
				.ToListAsync();
		}

		public async Task<Billing> GetBillingsById(int id)
		{
			return await _context.Billings
				.Include(s => s.BillingAddresses)
				.FirstOrDefaultAsync(s => s.Id == id) ?? new Billing();
		}

		public async Task<Billing> GetByIdAsync(int id)
		{
			return await _context.Billings.FirstOrDefaultAsync(s => s.Id == id) ?? new Billing();
		}

		public async Task<BillingAddress> GetByBillingIdAsync(int id)
		{
			return await _context.BillingAddresses.FirstOrDefaultAsync(s => s.BillingId == id) ?? new BillingAddress();
		}

		public void UpdateBillings(Billing billing)
		{
			_context.Billings.Update(billing);
		}

		public void UpdateBillingsAddress(BillingAddress billingAddress)
		{
			_context.BillingAddresses.Update(billingAddress);
		}

		public async Task<bool> RemoveBillings(int id)
		{
			var billing = await _context.Billings.Include(s => s.BillingAddresses).FirstOrDefaultAsync(s => s.Id == id);
			if (billing == null)
			{
				return false;
			}
			_context.BillingAddresses.RemoveRange(billing.BillingAddresses);
			_context.Billings.Remove(billing);
			return true;
		}

	}
}
