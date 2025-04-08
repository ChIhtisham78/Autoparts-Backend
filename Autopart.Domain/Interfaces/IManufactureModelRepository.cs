using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IManufactureModelRepository : IRepository<ManufacturerModel>
    {
        Task AddManufactureModelAsync(ManufacturerModel manufacturerModel);
        Task<IEnumerable<ManufacturerModel>> GetManufactureByManufacturerIdAsync(int? manufacturerId);
        Task DeleteByManufacturerIdAsync(int id);
        void AddModel(ManufacturerModel manufacturerModel);
        Task<ManufacturerModel?> GetModelByName(string name);

    }
}
