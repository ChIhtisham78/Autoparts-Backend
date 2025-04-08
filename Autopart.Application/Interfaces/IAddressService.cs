using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface IAddressService
    {
        Task<AddressDto> GetAddressById(int id);
        Task RemoveAddress(int id);
        Task UpdateAddress(AddressDto addressDto);
        Task<AddressDto> AddAddress(AddressDto addressDto);
        Task<List<AddressDto>> GetAddresses();
    }
}
