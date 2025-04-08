using Autopart.Application.Models;

namespace Autopart.Application.Interfaces
{
	public interface IRatingService
	{
		Task<ReviewDto> AddReview(ReviewDto reviewDto);
		Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId);

		Task<(IEnumerable<GetReviewsDto> Reviews, int TotalCount)> GetReviews(int? productId = null, string productSlug = null, int page = 1, int limit = 10);
		Task<bool> DeleteReviewAsync(int id);



    }
}
