using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class TempRepository : ITempRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public TempRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<List<Temp>> GetTemps()
        {
            return await _context.Temps.ToListAsync();
        }

        public async Task<Temp> GetTempById(int id)
        {
            return await _context.Temps.FirstOrDefaultAsync(x => x.Id == id);
        }

        public void AddTemp(Temp temp)
        {
            _context.Temps.Add(temp);
        }

        public void UpdateTemp(Temp temp)
        {
            _context.Temps.Update(temp);
        }

        public void DeleteTemp(Temp temp)
        {
            _context.Temps.Remove(temp);
        }
    }
}
