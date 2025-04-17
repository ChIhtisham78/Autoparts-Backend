using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class TaxRepository : ITaxRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public TaxRepository(autopartContext context)
        {
            _context = context;
        }

        public void AddTax(Tax tax)
        {
            _context.Taxes.Add(tax);
        }



        public async Task<List<Tax>> GetTax()
        {
          var getAllTax = await _context.Taxes.ToListAsync();
            return getAllTax ?? new List<Tax>();
        }



        public async Task<Tax> GetTaxById(int id)
        {
            var tax = await _context.Taxes.FindAsync(id);
            return tax ?? new Tax();
        }



        public async Task<Tax> GetByIdAsync(int id)
        {
            var tax = await _context.Taxes.FindAsync(id);
            return tax ?? new Tax();
        }

        public void UpdateTax(Tax tax)
        {
            _context.Taxes.Update(tax);
        }


        public async Task<Tax> GetTaxDelById(int id)
        {
            return await _context.Taxes.FindAsync(id);
        }

        public void RemoveTax(Tax tax)
        {
            _context.Taxes.Remove(tax);
        }


    }
}
