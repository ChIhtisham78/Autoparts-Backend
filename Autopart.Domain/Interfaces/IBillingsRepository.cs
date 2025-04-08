using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IBillingsRepository : IRepository<Billing>
	{
		void AddBillings(Billing billing);
		void AddBillingsAddress(BillingAddress billingAddress);
		Task<List<Billing>> GetBillings();
		Task<Billing> GetBillingsById(int id);
		Task<Billing> GetByIdAsync(int id);
		Task<BillingAddress> GetByBillingIdAsync(int id);
		void UpdateBillings(Billing billing);
		void UpdateBillingsAddress(BillingAddress billingAddress);
		Task<bool> RemoveBillings(int id);
	}
}
