using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Application.Models.Products;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class ShippingsService : IShippingsService
	{
		private readonly IShippingsRepository _shippingsRepository;
		private readonly ITypeAdapter _typeAdapter;

		public ShippingsService(IShippingsRepository shippingsRepository, ITypeAdapter typeAdapter)
		{
			_shippingsRepository = shippingsRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<ShippingsDto> AddShippings(ShippingsDto shippingsDto)
		{
			try
			{
				var shipping = new Shipping
				{
					OrderId = shippingsDto.OrderId,
					TrackingNo = shippingsDto.TrackingNo,
					Amount = shippingsDto.Amount,
					Type = shippingsDto.Type,
					Global = shippingsDto.Global,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
				};

				_shippingsRepository.AddShippings(shipping);
				await _shippingsRepository.UnitOfWork.SaveChangesAsync();

				var shippingAddress = new ShippingAddress
				{
					ShippingId = shipping.Id,
					Type = shippingsDto.ShippingAddressDto?.Type,
					IsDefault = shippingsDto.ShippingAddressDto?.IsDefault,
					Title = shippingsDto.ShippingAddressDto?.Title,
					OrderId = shippingsDto.OrderId,
					Zip = shippingsDto.ShippingAddressDto?.Zip,
					City = shippingsDto.ShippingAddressDto?.City,
					State = shippingsDto.ShippingAddressDto?.State,
					Country = shippingsDto.ShippingAddressDto?.Country,
					StreetAddress = shippingsDto.ShippingAddressDto?.StreetAddress,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
					UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
				};

				_shippingsRepository.AddShippingsAddress(shippingAddress);
				await _shippingsRepository.UnitOfWork.SaveChangesAsync();

				var shippingDto = _typeAdapter.Adapt<ShippingsDto>(shippingsDto);
				return shippingDto;
			}
			catch (Exception)
			{

				throw;
			}

		}




		public async Task<List<ShippingsDto>> GetShippings()
		{
			var shippings = await _shippingsRepository.GetShippings();
			var shippingDtos = _typeAdapter.Adapt<List<ShippingsDto>>(shippings);
			foreach (var dto in shippingDtos)
			{
				var firstShippingAddress = shippings.FirstOrDefault(s => s.Id == dto.Id)?.ShippingAddresses?.FirstOrDefault();
				if (firstShippingAddress != null)
				{
					dto.ShippingAddressDto = new ShippingAddressDto
					{
						Id = firstShippingAddress.Id,
						Zip = firstShippingAddress.Zip,
						City = firstShippingAddress.City,
						Title = firstShippingAddress.Title,
						Type = firstShippingAddress.Type,
						State = firstShippingAddress.State,
						Country = firstShippingAddress.Country,
						StreetAddress = firstShippingAddress.StreetAddress,
					};
				}
			}
			return shippingDtos;
		}


		public async Task<ShippingsDto> GetShippingsById(int id)
		{
			var shipping = await _shippingsRepository.GetShippingsById(id);
			var shippingDto = _typeAdapter.Adapt<ShippingsDto>(shipping);
			if (shipping.ShippingAddresses != null && shipping.ShippingAddresses.Any())
			{
				shippingDto.ShippingAddressDto = _typeAdapter.Adapt<ShippingAddressDto>(shipping.ShippingAddresses.First());
			}
			return shippingDto;
		}


		public async Task<ShippingsDto> UpdateShippings(int id, ShippingsDto shippingsDto)
		{
			try
			{
				var existingShipping = await _shippingsRepository.GetByIdAsync(id);
				if (existingShipping == null)
				{
					throw new KeyNotFoundException($"Shipping with ID {id} not found.");
				}

				existingShipping.OrderId = shippingsDto.OrderId;
				existingShipping.TrackingNo = shippingsDto.TrackingNo;
				existingShipping.Amount = shippingsDto.Amount;
				existingShipping.Type = shippingsDto.Type;
				existingShipping.Global = shippingsDto.Global;

				var existingShippingAddress = await _shippingsRepository.GetByShippingIdAsync(existingShipping.Id);
				if (existingShippingAddress == null)
				{
					throw new KeyNotFoundException($"Shipping Address for Shipping ID {existingShipping.Id} not found.");
				}

				existingShippingAddress.Type = shippingsDto.ShippingAddressDto?.Type;
				existingShippingAddress.IsDefault = shippingsDto.ShippingAddressDto?.IsDefault;
				existingShippingAddress.Title = shippingsDto.ShippingAddressDto?.Title;
				existingShippingAddress.OrderId = shippingsDto.OrderId;
				existingShippingAddress.Zip = shippingsDto.ShippingAddressDto?.Zip;
				existingShippingAddress.City = shippingsDto.ShippingAddressDto?.City;
				existingShippingAddress.State = shippingsDto.ShippingAddressDto?.State;
				existingShippingAddress.Country = shippingsDto.ShippingAddressDto?.Country;
				existingShippingAddress.StreetAddress = shippingsDto.ShippingAddressDto?.StreetAddress;

				_shippingsRepository.UpdateShippings(existingShipping);
				_shippingsRepository.UpdateShippingsAddress(existingShippingAddress);

				await _shippingsRepository.UnitOfWork.SaveChangesAsync();
				var shippingDto = _typeAdapter.Adapt<ShippingsDto>(existingShipping);
				return shippingDto;
			}
			catch (Exception ex)
			{

				throw;
			}

		}



		public async Task<bool> RemoveShippings(int id)
		{
			var result = await _shippingsRepository.RemoveShippings(id);
			if (!result)
			{
				return false;
			}

			await _shippingsRepository.UnitOfWork.SaveChangesAsync();
			return true;
		}

	}
}
