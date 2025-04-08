using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface ITaxService
    {
        Task<TaxDto> AddTax(TaxDto taxDto);
        Task<List<TaxDto>> GetTax();
        Task<TaxDto> GetTaxById(int id);
        Task<TaxDto> UpdateTax(int id, TaxDto taxDto);
        Task RemoveTax(int id);
    }
}
