using System.Collections.Generic;
using System.Threading.Tasks;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface IHomePageRepository : IRepository<HomePage>
    {
        Task<List<HomePage>> GetHomePagesAsync();
        Task<HomePage> GetByIdAsync(int id);
        void UpdateHomePage(HomePage homePage);
        void RemoveHomePage(HomePage homePage);
    }
}
