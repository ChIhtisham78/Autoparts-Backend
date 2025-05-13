using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class ManufactureModelRepository : IManufactureModelRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public ManufactureModelRepository(autopartContext context)
        {
            _context = context;
        }
        public async Task AddManufactureModelAsync(ManufacturerModel manufacturerModel)
        {
            await _context.ManufacturerModels.AddAsync(manufacturerModel);
        }
        public void AddModel(ManufacturerModel manufacturerModel)
        {
            _context.ManufacturerModels.Add(manufacturerModel);
        }
        public async Task<ManufacturerModel?> GetModelByName(string name)
        {
            return await _context.ManufacturerModels.FirstOrDefaultAsync(m => m.Model == name);
        }

        public async Task<IEnumerable<ManufacturerModel>> GetManufactureByManufacturerIdAsync(int? manufacturerId)
        {
            var query = from manufacturemodel in _context.ManufacturerModels
                        join manufacture in _context.Manufactures
                        on manufacturemodel.ManufacturerId equals manufacture.Id
                        where !manufacturerId.HasValue || manufacturemodel.ManufacturerId == manufacturerId.Value
                        select new ManufacturerModel
                        {
                            Id = manufacturemodel.Id,
                            Model = manufacturemodel.Model,
                            ManufacturerId = manufacturemodel.ManufacturerId,
                            Slug = manufacturemodel.Slug,
                            Manufacturer = new Manufacture
                            {
                                Id = manufacture.Id,
                                Name = manufacture.Name
                            }
                        };

            return await query.ToListAsync();
        }


        public async Task DeleteByManufacturerIdAsync(int id)
        {
            var manufacture = await _context.ManufacturerModels.FindAsync(id);

            if (manufacture == null)
            {
                throw new KeyNotFoundException("ManufacturerModel not found");
            }

            _context.ManufacturerModels.Remove(manufacture);
            await _context.SaveChangesAsync();
        }
    }
}
