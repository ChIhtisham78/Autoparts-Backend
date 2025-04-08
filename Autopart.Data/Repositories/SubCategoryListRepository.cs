using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class SubCategoryListRepository : ISubCategoryListRepository
    {
        private readonly autopartContext _context;
        public IUnitOfWork UnitOfWork => _context;
        public SubCategoryListRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task AddSubCategoryAsync(SubCategoryList subCategory)
        {
            await _context.SubCategoryLists.AddAsync(subCategory);
        }
        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public void AddSubCategory(SubCategoryList subCategoryList)
        {
            _context.SubCategoryLists.Add(subCategoryList);
        }
        public async Task<SubCategoryList?> GetsubCategoryByName(string name)
        {
            return await _context.SubCategoryLists.SingleOrDefaultAsync(m => m.Subcategory == name);
        }
        public async Task<IEnumerable<SubCategoryList>> GetSubCategoriesByCategoryIdAsync(int? categoryId)
        {
            var query = from sh in _context.SubCategoryLists
                        join c in _context.Categories
                        on sh.CategoryId equals c.Id
                        where !categoryId.HasValue || sh.CategoryId == categoryId.Value
                        select new SubCategoryList
                        {
                            Id = sh.Id,
                            Subcategory = sh.Subcategory,
                            CategoryId = sh.CategoryId,
                            Slug = sh.Slug,
                            Category = new Category
                            {
                                Id = c.Id,
                                Name = c.Name
                            }
                        };

            return await query.ToListAsync();
        }

        public async Task DeleteSubCategoryByIdAsync(int id)
        {
            var subCategory = await _context.SubCategoryLists.FindAsync(id);

            if (subCategory == null)
            {
                throw new KeyNotFoundException("Subcategory not found");
            }

            _context.SubCategoryLists.Remove(subCategory);
            await _context.SaveChangesAsync();
        }


    }
}
