using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
    public interface ISubCategoryListRepository : IRepository<SubCategoryList>
    {
        Task AddSubCategoryAsync(SubCategoryList subCategory);
        Task<IEnumerable<SubCategoryList>> GetSubCategoriesByCategoryIdAsync(int? categoryId);
        Task<Category> GetCategoryByIdAsync(int id);
        Task DeleteSubCategoryByIdAsync(int id);
        void AddSubCategory(SubCategoryList subCategoryList);
        Task<SubCategoryList?> GetsubCategoryByName(string name);

    }

}
