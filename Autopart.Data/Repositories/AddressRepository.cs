using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class AddressRepository : IAddressRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;

		public AddressRepository(autopartContext context)
		{
			_context = context;
		}


		public void AddAddress(Address address)
		{
			_context.Addresses.Add(address);
		}

		public void Updateaddress(Address address)
		{
			_context.Addresses.Update(address);
		}

		public void DeleteAddress(Address address)
		{
			_context.Addresses.Remove(address);
		}

		public async Task<List<Address>> GetAddresses()
		{
			return await _context.Addresses.ToListAsync();
		}

		public async Task<Address?> GetAddressById(int id)
		{
			return await _context.Addresses.FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}
