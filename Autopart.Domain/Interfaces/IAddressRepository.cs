using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IAddressRepository : IRepository<Address>
	{
		Task<Address?> GetAddressById(int id);
		Task<List<Address>> GetAddresses();
		void DeleteAddress(Address address);
		void Updateaddress(Address address);

		void AddAddress(Address address);

	}
}
