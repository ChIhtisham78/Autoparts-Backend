using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class SVCRelationRepository : ISVCRelationRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;


        public SVCRelationRepository(autopartContext context)
        {
            _context = context;
        }


        public async Task<Svcrelation> GetSVCRelationByIdAsync(int id)
        {
            var svcRelation = await _context.Svcrelations.FindAsync(id);
            return svcRelation ?? new Svcrelation();
        }


        public void UpdateSVCRelation(Svcrelation svcRelation)
        {
            _context.Svcrelations.Update(svcRelation);
        }



        public async Task<IEnumerable<Svcrelation>> GetSVCRelationAsync(int? shopId = null, string? size = null)
        {
            var query = _context.Svcrelations.AsQueryable();

            if (shopId.HasValue)
            {
                query = query.Where(s => s.ShopId == shopId);
            }

            if (!string.IsNullOrEmpty(size))
            {
                query = query.Where(s => s.Size == size);
            }

            return await query.ToListAsync();
        }

        public async Task AddSVCRelationsAsync(Svcrelation svcRelations)
        {
            _context.Svcrelations.AddRange(svcRelations);
            await _context.SaveChangesAsync();
        }
    }
}
