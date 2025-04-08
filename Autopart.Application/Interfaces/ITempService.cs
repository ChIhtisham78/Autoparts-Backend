using Autopart.Domain.Models;

namespace Autopart.Application.Interfaces
{
    public interface ITempService
    {
        Task<List<Temp>> GetTemps();

        Task<Temp> GetTempById(int id);

        Task<Temp> UpdateTemp(int id, string name);
        Task<Temp> AddTemp(string name);
        Task<Temp> RemoveTemp(int id);
    }
}
