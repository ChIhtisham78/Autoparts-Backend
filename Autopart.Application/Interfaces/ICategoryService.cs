using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface ICategoryService
	{
		Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
		Task<CategoryDto> GetCategoryByIdAsync(int id);
		Task<IEnumerable<CategoryDto>> GetCategoryBySlugAsync(string slug);
		Task<(IEnumerable<CategoryDto> Categories, int TotalCount)> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
		Task<IEnumerable<CategoryDto>> GetCategoriesByParamAsync(string param, string language);
		Task<IEnumerable<CategoryDto>> GetCategoriesByName(string param);
		Task UpdateCategoryAsync(CategoryDto categoryDto);
		Task DeleteCategoryAsync(int id);
	}
}
