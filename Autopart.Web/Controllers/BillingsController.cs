using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BillingsController : ControllerBase
	{
		private readonly IBillingsService _billingsService;

		public BillingsController(IBillingsService billingsService)
		{
			_billingsService = billingsService;
		}

		[Authorize]
		[HttpPost("billing")]
		public async Task<ActionResult> PostBillings([FromBody] BillingsDto billingsDto)
		{
			try
			{
				if (billingsDto == null)
				{
					return BadRequest("Billing data is null.");
				}

				var result = await _billingsService.AddBillings(billingsDto);
				return Ok(result);
			}
			catch (Exception)
			{

				throw;
			}


		}



		[HttpGet("billings")]
		public async Task<ActionResult> GetBillings()
		{
			var result = await _billingsService.GetBillings();
			return Ok(result);
		}


		[HttpGet("billing/{id}")]
		public async Task<ActionResult> GetBillingsById(int id)
		{
			return Ok(await _billingsService.GetBillingsById(id));
		}

		[Authorize]
		[HttpPut("billing")]
		public async Task<ActionResult> PutBillings(int id, [FromBody] BillingsDto billingsDto)
		{
			try
			{
				if (billingsDto == null)
				{
					return BadRequest("Billing data is null.");
				}

				var result = await _billingsService.UpdateBillings(id, billingsDto);
				return Ok(result);
			}
			catch (Exception)
			{

				throw;
			}

		}

		[Authorize]
		[HttpDelete("billing/{id}")]
		public async Task<ActionResult> DeleteBillings(int id)
		{
			await _billingsService.RemoveBillings(id);
			return Ok($"Billing with Id = {id} deleted successfully");
		}
	}
}
