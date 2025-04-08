using Autopart.Application.Models;
using Autopart.Data;
using Autopart.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RefundReasonController : ControllerBase
	{
		private readonly autopartContext _context;
		public RefundReasonController(autopartContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<IActionResult> GetRefundReasons()
		{
			var refundReasons = await _context.RefundReasons.ToListAsync();
			return Ok(new
			{
				data = refundReasons,
				total = refundReasons.Count,
				currentPage = 1,
				count = refundReasons.Count,
				lastPage = 1,
				firstItem = 0,
				lastItem = refundReasons.Count - 1,
				perPage = "10"
			});
		}


		[HttpGet("refund")]
		public IActionResult GetRefunds()
		{
			var refunds = new List<RefundResponse>();
			return Ok(refunds);
		}


		[HttpPost]
		public async Task<IActionResult> CreateRefundReason([FromBody] RefundReasonDto refundReasonDto)
		{
			try
			{
				var refundReason = new RefundReason
				{
					Name = refundReasonDto.Name,
					Slug = refundReasonDto.Slug,
					Language = refundReasonDto.Language,
					CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
					UpdatedAt = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified),
				};

				_context.RefundReasons.Add(refundReason);
				await _context.SaveChangesAsync();

				return Ok(refundReason);
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"An error occurred: {ex.Message}");
			}
		}


	}
}
