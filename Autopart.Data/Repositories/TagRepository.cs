using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class TagRepository : ITagRepository
	{
		private readonly autopartContext _context;

		public IUnitOfWork UnitOfWork => _context;

		public TagRepository(autopartContext context)
		{
			_context = context;
		}

		public async Task<Tag> CreateTagAsync(Tag tag)
		{
			await _context.Tags.AddAsync(tag);
			await _context.SaveChangesAsync();
			return tag;
		}

		public async Task<Tag> GetTagByIdAsync(int id)
		{
			return await _context.Tags.FindAsync(id);
		}

		public async Task<(IEnumerable<Tag> Tags, int TotalCount)> GetTagsAsync(string param, int pageNumber = 1, int pageSize = 10)
		{
			var query = _context.Tags.AsQueryable();

			if (!string.IsNullOrEmpty(param))
			{
				query = query.Where(t => t.Name.Contains(param));
			}

			var totalCount = await query.CountAsync();

			var tags = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

			return (tags, totalCount);
		}

		public async Task<IEnumerable<Tag>> GetTagByParamAsync(string param, string language)
		{
			return await _context.Tags.Where(x => x.Name.Contains(param) && x.Language.Contains(language)).ToListAsync();
		}

		public async Task<Tag> GetTagBySlug(string slug)
		{
			return await _context.Tags.Where(s => s.Slug == slug).FirstOrDefaultAsync();
		}





		public async Task<IEnumerable<Tag>> GetTagsByName(string name)
		{
			return await _context.Tags.Where(x => x.Name.Contains(name)).ToListAsync();
		}



		public async Task UpdateTagAsync(Tag tag)
		{
			_context.Tags.Update(tag);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveTagAsync(int id)
		{
			var tag = await _context.Tags.FindAsync(id);
			if (tag != null)
			{
				var productTags = await _context.ProductTags.Where(pt => pt.TagId == id).ToListAsync();

				_context.ProductTags.RemoveRange(productTags);

				_context.Tags.Remove(tag);

				await _context.SaveChangesAsync();
			}
		}

		public async Task<bool> SlugExistsAsync(string slug)
		{
			return await _context.Tags.AnyAsync(p => p.Slug == slug);
		}


	}
}