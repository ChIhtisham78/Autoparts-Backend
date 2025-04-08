using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
    public interface IHomePageService
    {
        Task<List<HomePageDto>> GetHomePagesAsync();
        Task<HomePageDto> GetHomePageByIdAsync(int id);
        Task<HomePageDto> UpdateHomePageAsync(int id, HomePageDto homePageDto);
        Task RemoveHomePageAsync(int id);
    }

}
