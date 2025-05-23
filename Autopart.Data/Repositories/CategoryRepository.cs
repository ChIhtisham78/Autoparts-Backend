using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly autopartContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public CategoryRepository(autopartContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            var categories = await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return categories.Entity;

        }
        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
        }

        public async Task<Category?> GetCategoryByName(string name)
        {
            var category = await _context.Categories.SingleOrDefaultAsync(m => m.Name == name);
            return category;
        }

        public async Task<Image> CreateImageAsync(Image image)
        {

            var images = await _context.Images.AddAsync(image);
            await _context.SaveChangesAsync();
            return images.Entity;

        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category!;
        }

        public async Task<(IEnumerable<Category> Categories, int TotalCount)> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Categories.AsQueryable();
            var totalCount = await query.CountAsync();

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var categories = await query.Include(p => p.Image).ToListAsync();

            return (categories, totalCount);
        }


        public async Task<IEnumerable<Category>> GetCategoriesByParamAsync(string param, string language)
        {
            var category = await _context.Categories.Where(x => x.Name.Contains(param) && x.Language.Contains(language)).ToListAsync();
            return category;

        }

        public async Task<IEnumerable<Category>> GetCategoriesByNameAsync(string param)
        {
               var category = await _context.Categories.Where(x => x.Name.Contains(param)).ToListAsync();
            return category;
                
        }

        public async Task<IEnumerable<Category>> GetCategoryBySlug(string slug)
        {
            return await _context.Categories.Where(x => x.Slug.Contains(slug)).ToListAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }


        public async Task<Image> GetImageByIdAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            return image!;
        }

        public async Task UpdateImageAsync(Image image)
        {
            _context.Images.Update(image);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }
        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteImageAsync(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Categories.AnyAsync(p => p.Slug == slug);
        }
    }
}
