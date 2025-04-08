using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class ManufactureModelService : IManufactureModelService
	{
		private readonly IManufactureModelRepository _manufactureModelRepository;
		private readonly ITypeAdapter _typeAdapter;

		public ManufactureModelService(IManufactureModelRepository manufactureModelRepository, ITypeAdapter typeAdapter)
		{
			_manufactureModelRepository = manufactureModelRepository;
			_typeAdapter = typeAdapter;
		}


		public async Task<AddManufactureModelDto> CreateManufactureModelAsync(AddManufactureModelDto manufacturerModel)
		{
			try
			{
				var manufacture = new ManufacturerModel
				{
					Id = manufacturerModel.Id,
					ManufacturerId = manufacturerModel.ManufacturerId,
					Model = manufacturerModel.Model,
					Slug = manufacturerModel.Slug
				};

				await _manufactureModelRepository.AddManufactureModelAsync(manufacture);
				await _manufactureModelRepository.UnitOfWork.SaveChangesAsync();
				return _typeAdapter.Adapt<AddManufactureModelDto>(manufacture);
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<IEnumerable<ManufactureModelDto>> GetManufactureByManufacturerIdAsync(int? manufacturerId)
		{
			var manufacture = await _manufactureModelRepository.GetManufactureByManufacturerIdAsync(manufacturerId);
			return _typeAdapter.Adapt<IEnumerable<ManufactureModelDto>>(manufacture);
		}

		public async Task DeleteByManufacturerIdAsync(int id)
		{
			await _manufactureModelRepository.DeleteByManufacturerIdAsync(id);
		}

	}
}
