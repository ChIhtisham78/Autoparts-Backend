using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Autopart.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class TaxController : ControllerBase
	{
		private readonly ITaxService _taxService;

		public TaxController(ITaxService taxService)
		{
			_taxService = taxService;
		}

		[Authorize]
		[HttpPost("tax")]
		public async Task<ActionResult> PostTax([FromBody] TaxDto taxDto)
		{
			if (taxDto == null)
			{
				return BadRequest("Shipping data is null.");
			}

			var result = await _taxService.AddTax(taxDto);
			return Ok(result);
		}


		[HttpGet("tax")]
		public async Task<ActionResult> GetTax()
		{
			var tax = await _taxService.GetTax();
			return Ok(tax);
		}


		[HttpGet("tax/{id}")]
		public async Task<ActionResult> GetTaxById(int id)
		{
			var taxbyId = await _taxService.GetTaxById(id);
			return Ok(taxbyId);
			
		}

		[Authorize]
		[HttpPut("tax/{id}")]
		public async Task<ActionResult> PutTax(int id, [FromBody] TaxDto taxDto)
		{
			if (taxDto == null)
			{
				return BadRequest("Tax data is null.");
			}

			var result = await _taxService.UpdateTax(id, taxDto);
			return Ok(result);
		}


		[Authorize]
		[HttpDelete("tax/{id}")]
		public async Task<ActionResult> DeleteTax(int id)
		{
			await _taxService.RemoveTax(id);
			return Ok($"Tax with Id = {id} deleted successfully");
		}

	}
}
