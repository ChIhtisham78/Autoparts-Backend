using System.Collections.Generic;
using System.Threading.Tasks;
using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Exceptions;
using Autopart.Domain.Interfaces;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
    public class HomePageService : IHomePageService
    {
        private readonly IHomePageRepository _homePageRepository;
        private readonly ITypeAdapter _typeAdapter;

        public HomePageService(IHomePageRepository homePageRepository, ITypeAdapter typeAdapter)
        {
            _homePageRepository = homePageRepository;
            _typeAdapter = typeAdapter;
        }

        public async Task<List<HomePageDto>> GetHomePagesAsync()
        {
            var homePages = await _homePageRepository.GetHomePagesAsync();
            return _typeAdapter.Adapt<List<HomePageDto>>(homePages);
        }

        public async Task<HomePageDto> GetHomePageByIdAsync(int id)
        {
            var homePage = await _homePageRepository.GetByIdAsync(id);
            if (homePage == null)
            {
                throw new DomainException("HomePage not found");
            }
            return _typeAdapter.Adapt<HomePageDto>(homePage);
        }

        public async Task<HomePageDto> UpdateHomePageAsync(int id, HomePageDto homePageDto)
        {
            var existingHomePage = await _homePageRepository.GetByIdAsync(id);
            if (existingHomePage == null)
            {
                throw new DomainException("HomePage not found");
            }

            existingHomePage.Title = homePageDto.Title;
            existingHomePage.Slug = homePageDto.Slug;
            existingHomePage.Description = homePageDto.Description;

            _homePageRepository.UpdateHomePage(existingHomePage);
            await _homePageRepository.UnitOfWork.SaveChangesAsync();
            return _typeAdapter.Adapt<HomePageDto>(existingHomePage);
        }

        public async Task RemoveHomePageAsync(int id)
        {
            var homePage = await _homePageRepository.GetByIdAsync(id);
            if (homePage == null)
            {
                throw new DomainException("HomePage not found");
            }
            _homePageRepository.RemoveHomePage(homePage);
            await _homePageRepository.UnitOfWork.SaveChangesAsync();
        }
    }
}
