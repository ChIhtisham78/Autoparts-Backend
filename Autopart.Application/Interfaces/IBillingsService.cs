using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface IBillingsService
	{
		Task<BillingsDto> AddBillings(BillingsDto billingsDto);
		Task<List<BillingsDto>> GetBillings();
		Task<BillingsDto> GetBillingsById(int id);
		Task<BillingsDto> UpdateBillings(int id, BillingsDto billingsDto);
		Task<bool> RemoveBillings(int id);
	}
}
