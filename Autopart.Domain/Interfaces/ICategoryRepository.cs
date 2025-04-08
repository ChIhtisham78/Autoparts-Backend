using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> CreateCategoryAsync(Category category);
        Task<Image> CreateImageAsync(Image image);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<(IEnumerable<Category> Categories, int TotalCount)> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10);
        Task<IEnumerable<Category>> GetCategoriesByParamAsync(string param, string language);
        Task<IEnumerable<Category>> GetCategoriesByNameAsync(string param);
        Task<IEnumerable<Category>> GetCategoryBySlug(string slug);
        Task UpdateCategoryAsync(Category category);
        Task<Image> GetImageByIdAsync(int id);
        Task<Category?> GetCategoryByName(string name);
        void AddCategory(Category category);
        Task UpdateImageAsync(Image image);
        Task DeleteCategoryAsync(int id);
        Task DeleteImageAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
        Task<bool> SlugExistsAsync(string slug);
    }
}
