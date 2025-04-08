using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class EngineRepository : IEngineRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public EngineRepository(autopartContext context)
        {
            _context = context;
        }


        public async Task AddEngineAsync(Engine engine)
        {
            await _context.Engines.AddAsync(engine);
        }

        public void AddEngine(Engine engine)
        {
            _context.Engines.Add(engine);
        }
        public async Task<Engine?> GetEngineByName(string name)
        {
            return await _context.Engines.SingleOrDefaultAsync(m => m.Engine1 == name);
        }
        public async Task<IEnumerable<Engine>> GetEnginesByParamsAsync(int? year, int? categoryId, int? subcategoryId, int? manufacturerId, int? modelId)
        {
            var query = _context.Engines.AsQueryable();

            if (year.HasValue)
            {
                query = query.Where(e => e.Year == year.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }
            if (subcategoryId.HasValue)
            {
                query = query.Where(e => e.SubcategoryId == subcategoryId.Value);
            }
            if (manufacturerId.HasValue)
            {
                query = query.Where(e => e.ManufacturerId == manufacturerId.Value);
            }
            if (modelId.HasValue)
            {
                query = query.Where(e => e.ModelId == modelId.Value);
            }
            return await query.ToListAsync();
        }


        public async Task<IEnumerable<EngineDto>> GetEnginesAsync(int? categoryId, int? subcategoryId, int? modelId, int? manufacturerId)
        {
            var query = from e in _context.Engines
                        join sc in _context.SubCategoryLists on e.SubcategoryId equals sc.Id into subcategorygrouped
                        from sc in subcategorygrouped.DefaultIfEmpty()
                        where !subcategoryId.HasValue || e.SubcategoryId == subcategoryId.Value
                        join m in _context.Manufactures on e.ManufacturerId equals m.Id into manufacturergrouped
                        from m in manufacturergrouped.DefaultIfEmpty()
                        where !manufacturerId.HasValue || e.ManufacturerId == manufacturerId.Value
                        join c in _context.Categories on e.CategoryId equals c.Id into categorygrouped
                        from c in categorygrouped.DefaultIfEmpty()
                        where !categoryId.HasValue || e.CategoryId == categoryId.Value
                        join mm in _context.ManufacturerModels on e.ModelId equals mm.Id into manufacturermodelgrouped
                        from mm in manufacturermodelgrouped.DefaultIfEmpty()
                        where !modelId.HasValue || e.ModelId == modelId.Value
                        select new EngineDto
                        {
                            Id = e.Id,
                            Year = e.Year,
                            ManufacturerId = e.ManufacturerId,
                            SubcategoryId = e.SubcategoryId,
                            CategoryId = e.CategoryId,
                            ModelId = e.ModelId,
                            Engine1 = e.Engine1,
                            HollanderCode = e.HollanderCode,
                            CategoryName = c != null ? c.Name : string.Empty,
                            SubcategoryName = sc != null ? sc.Subcategory : string.Empty,
                            ManufacturerName = m != null ? m.Name : string.Empty,
                            ModelName = mm != null ? mm.Model : string.Empty
                        };

            return await query.ToListAsync();
        }


        public async Task DeleteEngineByIdAsync(int id)
        {
            var engine = await _context.Engines.FindAsync(id);

            if (engine == null)
            {
                throw new KeyNotFoundException("engine not found");
            }

            _context.Engines.Remove(engine);
            await _context.SaveChangesAsync();
        }
    }
}
