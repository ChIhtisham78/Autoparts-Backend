using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class AddressService : IAddressService
	{
		private readonly IAddressRepository _addressRepository;
		private readonly ITypeAdapter _typeAdapter;

		public AddressService(IAddressRepository addressRepository, ITypeAdapter typeAdapter)
		{
			_addressRepository = addressRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<AddressDto> GetAddressById(int id)
		{
			var adrress = _typeAdapter.Adapt<AddressDto>(await _addressRepository.GetAddressById(id));
			return adrress;
		}

		public async Task RemoveAddress(int id)
		{
			var address = await _addressRepository.GetAddressById(id);
			if (address == null)
			{
				throw new DomainException("temp not exists");
			}
			_addressRepository.DeleteAddress(address);
			await _addressRepository.UnitOfWork.SaveChangesAsync();

		}


		public async Task UpdateAddress(AddressDto addressDto)
		{
			var address = await _addressRepository.GetAddressById(addressDto.Id);
			if (address == null)
			{
				throw new DomainException("User not exists");
			}
			address.Id = addressDto.Id;
			address.Zip = addressDto.Zip;
			address.City = addressDto.City;
			address.Title = addressDto.Title;
			address.Type = addressDto.Type;
			address.State = addressDto.State;
			address.Country = addressDto.Country;
			address.StreetAddress = addressDto.StreetAddress;

			_addressRepository.Updateaddress(address);
			await _addressRepository.UnitOfWork.SaveChangesAsync();

		}

		public async Task<AddressDto> AddAddress(AddressDto addressDto)
		{
			try
			{
				var address = new Address();

				address.UserId = addressDto.UserId;
				address.StreetAddress = addressDto.StreetAddress;
				address.City = addressDto.City;
				address.State = addressDto.State;
				address.Country = addressDto.Country;
				address.Zip = addressDto.Zip;
				address.Type = addressDto.Type;
				address.Title = addressDto.Title;
				address.IsDefault = 1;
				_addressRepository.AddAddress(address);
				await _addressRepository.UnitOfWork.SaveChangesAsync();

				var addressdto = _typeAdapter.Adapt<AddressDto>(address);
				return addressdto;
			}
			catch (Exception)
			{

				throw;
			}
		}


		public async Task<List<AddressDto>> GetAddresses()
		{
			var address = await _addressRepository.GetAddresses();
			var addressDto = _typeAdapter.Adapt<List<AddressDto>>(address);
			return addressDto;
		}
	}
}
