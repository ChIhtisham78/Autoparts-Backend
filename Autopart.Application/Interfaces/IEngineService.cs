using Autopart.Application.Models;
using Autopart.Domain.Interfaces;

namespace Autopart.Application.Interfaces
{
	public interface IEngineService
	{
		Task<AddEngineDto> CreateEngineAsync(AddEngineDto engineDto);
		Task<IEnumerable<EngineDto>> GetEnginesAsync(int? categoryId, int? subcategoryId, int? modelId, int? manufacturerId);
		Task<IEnumerable<EngineIdAndNameDto>> GetEnginesByParamsAsync(int? year, int? categoryId, int? subcategoryId, int? manufacturerId, int? modelId);
		Task DeleteEngineByIdAsync(int id);
	}
}
