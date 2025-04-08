using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class EngineService : IEngineService
	{
		private readonly IEngineRepository _engineRepository;
		private readonly ITypeAdapter _typeAdapter;

		public EngineService(IEngineRepository engineRepository, ITypeAdapter typeAdapter)
		{
			_engineRepository = engineRepository;
			_typeAdapter = typeAdapter;
		}


		public async Task<AddEngineDto> CreateEngineAsync(AddEngineDto engineDto)
		{
			try
			{
				var engine = new Engine
				{
					Id = engineDto.Id,
					CategoryId = engineDto.CategoryId,
					SubcategoryId = engineDto.SubcategoryId,
					ManufacturerId = engineDto.ManufacturerId,
					ModelId = engineDto.ModelId,
					Year = engineDto.Year,
					Engine1 = engineDto.Engine1,
					HollanderCode = engineDto.HollanderCode,

				};

				await _engineRepository.AddEngineAsync(engine);
				await _engineRepository.UnitOfWork.SaveChangesAsync();
				return _typeAdapter.Adapt<AddEngineDto>(engine);
			}
			catch (Exception)
			{
				throw;
			}
		}


		public async Task<IEnumerable<EngineIdAndNameDto>> GetEnginesByParamsAsync(int? year, int? categoryId, int? subcategoryId, int? manufacturerId, int? modelId)
		{
			var engines = await _engineRepository.GetEnginesByParamsAsync(year, categoryId, subcategoryId, manufacturerId, modelId);
			return _typeAdapter.Adapt<IEnumerable<EngineIdAndNameDto>>(engines);
		}



		public async Task<IEnumerable<EngineDto>> GetEnginesAsync(int? categoryId, int? subcategoryId, int? modelId, int? manufacturerId)
		{
			var engines = await _engineRepository.GetEnginesAsync(categoryId, subcategoryId, modelId, manufacturerId);
			return _typeAdapter.Adapt<IEnumerable<EngineDto>>(engines);
		}

		public async Task DeleteEngineByIdAsync(int id)
		{
			await _engineRepository.DeleteEngineByIdAsync(id);
		}
	}
}
