using Autopart.Domain.CommonDTO;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IEngineRepository : IRepository<Engine>
    {
        Task AddEngineAsync(Engine engine);
        Task<IEnumerable<EngineDto>> GetEnginesAsync(GetEnginesDTO enginesDTO);
        Task<IEnumerable<Engine>> GetEnginesByParamsAsync(GetEnginesDTO enginesDTO);
        void AddEngine(Engine engine);
        Task<Engine?> GetEngineByName(string name);
        Task DeleteEngineByIdAsync(int id);
    }
}
