using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Domain.Interfaces
{
	public interface IRatingRepository : IRepository<Rating>
	{
		void AddRating(Rating rating);
		Task<(IEnumerable<Rating> Ratings, int TotalCount)> GetRatingsWithProduct(int? productId = null, string productSlug = null, int page = 1, int limit = 10);
		Task<IEnumerable<Rating>> GetReviewsByProductIdAsync(int productId);


		Task<Rating> GetByIdAsync(int id);
		void Delete(Rating review);

	}

}
