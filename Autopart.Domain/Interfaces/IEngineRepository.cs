using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IEngineRepository : IRepository<Engine>
    {
        Task AddEngineAsync(Engine engine);
        Task<IEnumerable<EngineDto>> GetEnginesAsync(int? categoryId, int? subcategoryId, int? modelId, int? manufacturerId);
        Task<IEnumerable<Engine>> GetEnginesByParamsAsync(int? year, int? categoryId, int? subcategoryId, int? manufacturerId, int? modelId);
        void AddEngine(Engine engine);
        Task<Engine?> GetEngineByName(string name);

        Task DeleteEngineByIdAsync(int id);

    }
}
