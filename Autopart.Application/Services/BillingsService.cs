using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class BillingsService : IBillingsService
	{
		private readonly IBillingsRepository _billingsRepository;
		private readonly ITypeAdapter _typeAdapter;

		public BillingsService(IBillingsRepository billingsRepository, ITypeAdapter typeAdapter)
		{
			_billingsRepository = billingsRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<BillingsDto> AddBillings(BillingsDto billingsDto)
		{
			try
			{
				var billing = new Billing
				{
					OrderId = billingsDto.OrderId,
					TrackingNo = billingsDto.TrackingNo,
					Amount = billingsDto.Amount,
					Type = billingsDto.Type,
					Global = billingsDto.Global,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
				};

				_billingsRepository.AddBillings(billing);
				await _billingsRepository.UnitOfWork.SaveChangesAsync();

				var billingAddress = new BillingAddress
				{
					BillingId = billing.Id,
					Type = billingsDto.BillingsAddressDto?.Type,
					IsDefault = billingsDto.BillingsAddressDto?.IsDefault,
					Title = billingsDto.BillingsAddressDto?.Title,
					OrderId = billingsDto.OrderId,
					Zip = billingsDto.BillingsAddressDto?.Zip,
					City = billingsDto.BillingsAddressDto?.City,
					State = billingsDto.BillingsAddressDto?.State,
					Country = billingsDto.BillingsAddressDto?.Country,
					StreetAddress = billingsDto.BillingsAddressDto?.StreetAddress,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
					UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified)
				};

				_billingsRepository.AddBillingsAddress(billingAddress);
				await _billingsRepository.UnitOfWork.SaveChangesAsync();

				var billingDto = _typeAdapter.Adapt<BillingsDto>(billingsDto);
				return billingDto;
			}
			catch (Exception)
			{
				throw;
			}

		}




		public async Task<List<BillingsDto>> GetBillings()
		{
			var billings = await _billingsRepository.GetBillings();
			var billingsDtos = _typeAdapter.Adapt<List<BillingsDto>>(billings);
			foreach (var dto in billingsDtos)
			{
				var firstBillingAddress = billings.FirstOrDefault(s => s.Id == dto.Id)?.BillingAddresses?.FirstOrDefault();
				if (firstBillingAddress != null)
				{
					dto.BillingsAddressDto = new BillingsAddressDto
					{
						Id = firstBillingAddress.Id,
						Zip = firstBillingAddress.Zip,
						City = firstBillingAddress.City,
						Title = firstBillingAddress.Title,
						Type = firstBillingAddress.Type,
						State = firstBillingAddress.State,
						Country = firstBillingAddress.Country,
						StreetAddress = firstBillingAddress.StreetAddress,
					};
				}
			}
			return billingsDtos;
		}


		public async Task<BillingsDto> GetBillingsById(int id)
		{
			var billing = await _billingsRepository.GetBillingsById(id);
			var billingDto = _typeAdapter.Adapt<BillingsDto>(billing);
			if (billing.BillingAddresses != null && billing.BillingAddresses.Any())
			{
				billingDto.BillingsAddressDto = _typeAdapter.Adapt<BillingsAddressDto>(billing.BillingAddresses.First());
			}
			return billingDto;
		}


		public async Task<BillingsDto> UpdateBillings(int id, BillingsDto billingsDto)
		{
			try
			{
				var existingBilling = await _billingsRepository.GetByIdAsync(id);
				if (existingBilling == null)
				{
					throw new KeyNotFoundException($"Billings with ID {id} not found.");
				}

				existingBilling.OrderId = billingsDto.OrderId;
				existingBilling.TrackingNo = billingsDto.TrackingNo;
				existingBilling.Amount = billingsDto.Amount;
				existingBilling.Type = billingsDto.Type;
				existingBilling.Global = billingsDto.Global;

				var existingBillingAddress = await _billingsRepository.GetByBillingIdAsync(existingBilling.Id);
				if (existingBillingAddress == null)
				{
					throw new KeyNotFoundException($"Billing Address for Billing ID {existingBilling.Id} not found.");
				}

				existingBillingAddress.Type = billingsDto.BillingsAddressDto?.Type;
				existingBillingAddress.IsDefault = billingsDto.BillingsAddressDto?.IsDefault;
				existingBillingAddress.Title = billingsDto.BillingsAddressDto?.Title;
				existingBillingAddress.OrderId = billingsDto.OrderId;
				existingBillingAddress.Zip = billingsDto.BillingsAddressDto?.Zip;
				existingBillingAddress.City = billingsDto.BillingsAddressDto?.City;
				existingBillingAddress.State = billingsDto.BillingsAddressDto?.State;
				existingBillingAddress.Country = billingsDto.BillingsAddressDto?.Country;
				existingBillingAddress.StreetAddress = billingsDto.BillingsAddressDto?.StreetAddress;

				_billingsRepository.UpdateBillings(existingBilling);
				_billingsRepository.UpdateBillingsAddress(existingBillingAddress);

				await _billingsRepository.UnitOfWork.SaveChangesAsync();
				var billingDto = _typeAdapter.Adapt<BillingsAddressDto>(existingBilling);
				return billingsDto;
			}
			catch (Exception ex)
			{

				throw;
			}

		}


		public async Task<bool> RemoveBillings(int id)
		{
			var result = await _billingsRepository.RemoveBillings(id);
			if (!result)
			{
				return false;
			}
			await _billingsRepository.UnitOfWork.SaveChangesAsync();
			return true;
		}

	}
}
