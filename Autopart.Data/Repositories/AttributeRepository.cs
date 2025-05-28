using Autopart.Domain.Interfaces;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly autopartContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public AttributeRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<Domain.Models.Attribute> CreateAttributeAsync(Domain.Models.Attribute attribute)
        {
            await _context.Attributes.AddAsync(attribute);
            await _context.SaveChangesAsync();
            return attribute;
        }
        public async Task<Domain.Models.Value> CreateValueAsync(Domain.Models.Value value)
        {
            await _context.Values.AddAsync(value);
            await _context.SaveChangesAsync();
            return value;
        }

        public async Task<Domain.Models.Attribute> GetAttributeByIdAsync(int id)
        {
           var attribute = await _context.Attributes.Include(x=>x.Values).FirstOrDefaultAsync(x=> x.Id == id);
            return attribute ?? new Domain.Models.Attribute();
        }

        public async Task<IEnumerable<Domain.Models.Attribute>> GetAttributesAsync(int? shopId)
        {
            var query = _context.Attributes.Include(a => a.Values).AsQueryable();

            if (shopId.HasValue)
            {
                query = query.Where(a => a.ShopId == shopId.Value);
            }
            return await query.ToListAsync();
        }


        public async Task<IEnumerable<Domain.Models.Attribute>> GetAttributeByParamAsync(string param)
        {
            var getAttributes = await _context.Attributes.Where(e => e.Name.Contains(param)).ToListAsync();
            return getAttributes;
        }

        public async Task UpdateAttributeAsync(Domain.Models.Attribute attribute)
        {
            _context.Attributes.Update(attribute);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateValueAsync(Domain.Models.Value value)
        {
            _context.Values.Update(value);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAttributeAsync(int id)
        {
            var attribute = await _context.Attributes.FindAsync(id);
            if (attribute != null)
            {
                _context.Attributes.Remove(attribute);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveValueAsync(int id)
        {
            var value = await _context.Values.FindAsync(id);
            if (value != null)
            {
                _context.Values.Remove(value);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveValueByAttributeAsync(int attributeId)
        {
            var valuesToRemove = await _context.Values.Where(v => v.AttributeId == attributeId).ToListAsync();

            if (valuesToRemove != null && valuesToRemove.Any())
            {
                _context.Values.RemoveRange(valuesToRemove);
                await _context.SaveChangesAsync();
            }
        }

    }
}
