using Autopart.Application.Models;
using Autopart.Domain.CommonDTO;
using Autopart.Domain.Interfaces;

namespace Autopart.Application.Interfaces
{
	public interface IEngineService
	{
		Task<AddEngineDto> CreateEngineAsync(AddEngineDto engineDto);
		Task<IEnumerable<EngineDto>> GetEnginesAsync(GetEnginesDTO enginesDTO);
		Task<IEnumerable<EngineIdAndNameDto>> GetEnginesByParamsAsync(GetEnginesDTO enginesDTO);
		Task DeleteEngineByIdAsync(int id);
	}
}
