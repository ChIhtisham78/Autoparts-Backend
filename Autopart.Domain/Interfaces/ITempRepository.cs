using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ITempRepository : IRepository<Temp>
    {
        Task<List<Temp>> GetTemps();
        Task<Temp> GetTempById(int id);
        void AddTemp(Temp temp);
        void UpdateTemp(Temp temp);
        void DeleteTemp(Temp temp);
    }
}
