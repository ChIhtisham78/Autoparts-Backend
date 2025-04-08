using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
    public class TaxSevice : ITaxService
    {
        private readonly ITaxRepository _taxRepository;
        private readonly ITypeAdapter _typeAdapter;

        public TaxSevice(ITaxRepository taxRepository, ITypeAdapter typeAdapter)
        {
            _taxRepository = taxRepository;
            _typeAdapter = typeAdapter;
        }

        public async Task<TaxDto> AddTax(TaxDto taxDto)
        {
            var tax = new Tax
            {
                Name = taxDto.Name,
                Rate = taxDto.Rate,
                State = taxDto.State,
                Zip = taxDto.Zip,
                City = taxDto.City,
                Priority = taxDto.Priority,
                OnShipping = taxDto.OnShipping,
            };

            _taxRepository.AddTax(tax);
            await _taxRepository.UnitOfWork.SaveChangesAsync();
            var taxdto = _typeAdapter.Adapt<TaxDto>(taxDto);
            return taxdto;
        }


        public async Task<List<TaxDto>> GetTax()
        {
            var tax = await _taxRepository.GetTax();
            var taxDto = _typeAdapter.Adapt<List<TaxDto>>(tax);
            return taxDto;
        }


        public async Task<TaxDto> GetTaxById(int id)
        {
            var tax = _typeAdapter.Adapt<TaxDto>(await _taxRepository.GetTaxById(id));
            return tax;
        }


        public async Task<TaxDto> UpdateTax(int id, TaxDto taxDto)
        {
            try
            {
                var existingtax = await _taxRepository.GetByIdAsync(id);
                if (existingtax == null)
                {
                    throw new Exception("Tax not found");
                }
                existingtax.Name = taxDto.Name;
                existingtax.Rate = taxDto.Rate;
                existingtax.State = taxDto.State;
                existingtax.Zip = taxDto.Zip;
                existingtax.City = taxDto.City;
                existingtax.Priority = taxDto.Priority;
                existingtax.OnShipping = taxDto.OnShipping;

                _taxRepository.UpdateTax(existingtax);
                await _taxRepository.UnitOfWork.SaveChangesAsync();
                var taxdto = _typeAdapter.Adapt<TaxDto>(existingtax);
                return taxdto;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task RemoveTax(int id)
        {
            var tax = await _taxRepository.GetTaxDelById(id);
            if (tax == null)
            {
                throw new DomainException("Tax not exists");
            }
            _taxRepository.RemoveTax(tax);
            await _taxRepository.UnitOfWork.SaveChangesAsync();
        }

    }
}
