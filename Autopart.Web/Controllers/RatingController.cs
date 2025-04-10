using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RatingController : ControllerBase
	{
		private readonly IRatingService _ratingService;

		public RatingController(IRatingService ratingService)
		{
			_ratingService = ratingService;
		}


		[HttpGet("review/{productId}")]
		public async Task<IActionResult> GetReviewsByProductId(int productId)
		{
			var reviews = await _ratingService.GetReviewsByProductIdAsync(productId);
			if (reviews == null || !reviews.Any())
			{
				return Ok(new List<ReviewDto>());
			}

			return Ok(reviews);
		}

		//[Authorize]
		[HttpPost("review")]
		public async Task<ActionResult> PostReview(ReviewDto reviewDto)
		{
			var postReview = await _ratingService.AddReview(reviewDto);
			return Ok(postReview);
		}
		
		
		[HttpGet("review")]
		public async Task<ActionResult> GetReviews(int? productId = null, string productSlug = null, int page = 1, int limit = 10)
		{
			var (reviews, totalCount) = await _ratingService.GetReviews(productId, productSlug, page, limit);

			var response = new
			{
				result = reviews ?? new List<GetReviewsDto>(),
				total = totalCount,
				currentPage = page,
				count = reviews?.Count() ?? 0,
				lastPage = (int)Math.Ceiling((double)totalCount / limit),
				firstItem = (page - 1) * limit + 1,
				lastItem = (page - 1) * limit + (reviews?.Count() ?? 0),
				perPage = limit,
			};

			return new JsonResult(response)
			{
				StatusCode = (int)HttpStatusCode.OK
			};
		}

        [HttpDelete("review/{id}")]
		public async Task<IActionResult> DeleteReview(int id)
		{
			var result = await _ratingService.DeleteReviewAsync(id);
			if (!result)
			{
				return NotFound(new { Message = "Review not found." });
			}

			return Ok(result);
		}
		
	}
}
