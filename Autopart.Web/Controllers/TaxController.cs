using Autopart.Application.Interfaces;
using Autopart.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
				return BadRequest("Tax data is null.");
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

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return NotFound();

			var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
			Directory.CreateDirectory(uploadsFolder);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
				await file.CopyToAsync(stream);
            return Ok(new { filePath });

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
