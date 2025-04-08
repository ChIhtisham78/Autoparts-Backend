using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class SubCategoryListService : ISubCategoryListService
	{
		private readonly ISubCategoryListRepository _subCategoryListRepository;
		private readonly ITypeAdapter _typeAdapter;

		public SubCategoryListService(ISubCategoryListRepository subCategoryListRepository, ITypeAdapter typeAdapter)
		{
			_subCategoryListRepository = subCategoryListRepository;
			_typeAdapter = typeAdapter;
		}

		public async Task<AddSubCategoryListDto> CreateSubCategoryAsync(AddSubCategoryListDto dto)
		{
			try
			{
				var subCategory = new SubCategoryList
				{
					Id = dto.Id,
					CategoryId = dto.CategoryId,
					Slug = dto.Slug
				};

				await _subCategoryListRepository.AddSubCategoryAsync(subCategory);
				await _subCategoryListRepository.UnitOfWork.SaveChangesAsync();
				return _typeAdapter.Adapt<AddSubCategoryListDto>(subCategory);
			}
			catch (Exception ex)
			{
				throw;
			}
		}


		public async Task<IEnumerable<SubCategoryListDto>> GetSubCategoriesByCategoryIdAsync(int? categoryId)
		{
			var subCategories = await _subCategoryListRepository.GetSubCategoriesByCategoryIdAsync(categoryId);
			return _typeAdapter.Adapt<IEnumerable<SubCategoryListDto>>(subCategories);
		}

		public async Task DeleteSubCategoryByIdAsync(int id)
		{
			await _subCategoryListRepository.DeleteSubCategoryByIdAsync(id);
		}

	}
}
