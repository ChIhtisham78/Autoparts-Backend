using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Data.Repositories
{
	public class RatingRepository : IRatingRepository
	{
		private readonly autopartContext _context;
		public IUnitOfWork UnitOfWork => _context;
		public RatingRepository(autopartContext context)
		{
			_context = context;
		}

		public void AddRating(Rating rating)
		{
			_context.Ratings.Add(rating);
		}
	


		public async Task<IEnumerable<Rating>> GetReviewsByProductIdAsync(int productId)
		{
			return await _context.Ratings
				.Where(r => r.ProductId == productId)
				.ToListAsync();
		}

		public async Task<Rating> GetByIdAsync(int id)
		{
			return await _context.Ratings.FindAsync(id) ?? new Rating();
		}

		public void Delete(Rating review)
		{
			_context.Ratings.Remove(review);
		}



		//public async Task<List<Rating>> GetRatingsWithProduct()
		//{
		//    return await _context.Ratings
		//        .Include(r => r.Product).Include(r => r.Order).ThenInclude(o => o.Customer).ToListAsync();
		//}
		public async Task<(IEnumerable<Rating> Ratings, int TotalCount)> GetRatingsWithProduct(int? productId = null, string productSlug = null, int page = 1, int limit = 10)
		{
			var query = _context.Ratings
				.Include(r => r.Product)
				.Include(r => r.Order).ThenInclude(o => o.Customer)
				.AsQueryable();

			if (productId.HasValue)
			{
				query = query.Where(r => r.ProductId == productId.Value);
			}
			if (!string.IsNullOrEmpty(productSlug))
			{
				query = query.Where(r => r.Product.Slug == productSlug);
			}

			var totalCount = await query.CountAsync();

			var ratings = await query
				.Skip((page - 1) * limit)
				.Take(limit)
				.ToListAsync();

			return (ratings, totalCount);
		}

       


    }
}
