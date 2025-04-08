using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ITaxRepository : IRepository<Tax>
    {
        void AddTax(Tax tax);
        Task<List<Tax>> GetTax();

        Task<Tax> GetByIdAsync(int id);
        Task<Tax> GetTaxById(int id);
        void UpdateTax(Tax tax);
        void RemoveTax(Tax tax);
        Task<Tax> GetTaxDelById(int id);
    }
}
