using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface IManufactureModelService
	{
		Task<AddManufactureModelDto> CreateManufactureModelAsync(AddManufactureModelDto manufacturerModel);
		Task<IEnumerable<ManufactureModelDto>> GetManufactureByManufacturerIdAsync(int? manufacturerId);
		Task DeleteByManufacturerIdAsync(int id);


	}
}
