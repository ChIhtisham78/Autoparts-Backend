using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface ISubCategoryListService
	{
		Task<AddSubCategoryListDto> CreateSubCategoryAsync(AddSubCategoryListDto dto);
		Task<IEnumerable<SubCategoryListDto>> GetSubCategoriesByCategoryIdAsync(int? categoryId);
		Task DeleteSubCategoryByIdAsync(int id);

	}

}
