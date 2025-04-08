using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Autopart.Domain.Interfaces;
using Autopart.Domain.Models;
using Autopart.Domain.SharedKernel;

namespace Autopart.Application.Services
{
	public class RatingService : IRatingService
	{
		private readonly IRatingRepository _ratingRepository;
		private readonly ITypeAdapter _typeAdapter;

		public RatingService(IRatingRepository ratingRepository, ITypeAdapter typeAdapter)
		{
			_ratingRepository = ratingRepository;
			_typeAdapter = typeAdapter;

		}
		public async Task<IEnumerable<ReviewDto>> GetReviewsByProductIdAsync(int productId)
		{
			var reviews = await _ratingRepository.GetReviewsByProductIdAsync(productId);
			return _typeAdapter.Adapt<IEnumerable<ReviewDto>>(reviews);
		}


		public async Task<ReviewDto> AddReview(ReviewDto reviewDto)
		{
			try
			{
				var rating = new Rating()
				{
					ProductId = reviewDto.ProductId,
					OrderId = reviewDto.OrderId,
					Rating1 = reviewDto.Rating,
					Comments = reviewDto.Comments
				};


				_ratingRepository.AddRating(rating);
				await _ratingRepository.UnitOfWork.SaveChangesAsync();

				var reviewsDto = _typeAdapter.Adapt<ReviewDto>(rating);
				return reviewsDto;
			}
			catch (Exception ex)
			{
				throw new Exception("Failed to invalidate JWT token.", ex);

			}


		}




		public async Task<(IEnumerable<GetReviewsDto> Reviews, int TotalCount)> GetReviews(int? productId = null, string productSlug = null, int page = 1, int limit = 10)
		{
			var (ratings, totalCount) = await _ratingRepository.GetRatingsWithProduct(productId, productSlug, page, limit);

			var reviewDtos = ratings.Select(r => new GetReviewsDto
			{
				Id = r.Id,
				OrderId = r.OrderId,
				UserId = r.Order?.CustomerId ?? 0,
				ShopId = r.Product.ShopId ?? 0,
				ProductId = r.ProductId,
				Comment = r.Comments,
				Reports = 0,
				Rating = r.Rating1,
				PositiveFeedbacksCount = r.PositiveFeedbacksCount ?? 0,
				NegativeFeedbacksCount = r.NegativeFeedbacksCount ?? 0,
				MyFeedback = r.MyFeedback,
				AbusiveReportsCount = r.AbusiveReportsCount ?? 0,
				CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),

				Product = new RatingProductsDto
				{
					Id = r.Product.Id,
					Name = r.Product.Name,
					Slug = r.Product.Slug,
					Description = r.Product.Description,
					TypeId = r.Product.TypeId ?? 0,
					Price = r.Product.Price ?? 0,
					ShopId = r.Product.ShopId ?? 0,
					SalePrice = r.Product.SalePrice ?? 0,
					Language = r.Product.Language,
					MinPrice = r.Product.MinPrice ?? 0,
					MaxPrice = r.Product.MaxPrice ?? 0,
					Sku = r.Product.Sku,
					Quantity = r.Product.Quantity ?? 0
				},
				User = r.Order?.Customer != null ? new RatingUsersDto
				{
					Id = r.Order.CustomerId ?? 0,
					Name = r.Order.Customer.UserName,
					Email = r.Order.Customer.Email,
					CreatedAt = r.Order.Customer.CreatedAt,
					UpdatedAt = r.Order.Customer.UpdatedAt,
					IsActive = r.Order.Customer.IsActive,
				} : null
			}).ToList();

			return (reviewDtos, totalCount);
		}








		public async Task<bool> DeleteReviewAsync(int id)
		{
			var review = await _ratingRepository.GetByIdAsync(id);
			if (review == null)
			{
				return false;
			}

			_ratingRepository.Delete(review);
			await _ratingRepository.UnitOfWork.SaveChangesAsync();
			return true;
		}



	}
}
