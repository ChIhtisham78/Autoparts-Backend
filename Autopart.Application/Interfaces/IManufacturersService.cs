using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface IManufacturersService
	{
		Task<(List<ManufacturersDto> Manufacturers, int TotalCount)> GetManufacturers(int pageNumber = 1, int pageSize = 10);
		Task<ManufacturersDto> GetManufacturerById(int id);
		Task<ManufacturersDto> GetManufacturerBySlugAsync(string slug);

		Task RemoveManufacturer(int id);
		Task<ManufacturersDto> AddManufacturers(ManufacturersDto manufacturersDto, ImageDto imageDto, SocialDto socialDto/* TypeDto typeDto, BannerDto bannerDto*/);
		Task UpdateManufacturers(ManufacturersDto manufacturersDto, SocialDto socialDto, ImageDto imageDto/*, TypeDto typeDto, BannerDto bannerDto*/);

	}
}
